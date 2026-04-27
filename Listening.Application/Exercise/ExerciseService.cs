using Listening.Application.Dtos;
using Listening.Application.Exercise.Commands;
using Listening.Application.Interfaces;
using Listening.Domain.Entities;
using TagLib;
using System.IO;
using Listening.Application.Mappings;
using File = System.IO.File;

namespace Listening.Application.Exercise;

public class ExerciseService
{
    private readonly ILessonRepository _lessonRepo;
    private readonly IFileStorage _fileStorage;
    private readonly ISubtitleParser _subtitleParser;

    public ExerciseService(
        ILessonRepository lessonRepo,
        IFileStorage fileStorage,
        ISubtitleParser subtitleParser)
    {
        _lessonRepo = lessonRepo;
        _fileStorage = fileStorage;
        _subtitleParser = subtitleParser;
    }

    public async Task<ExerciseDto> CreateExerciseAsync(CreateExerciseCommand cmd)
    {
        var lessonExists = await _lessonRepo.ExistsAsync(cmd.LessonId);
        if (!lessonExists)
            throw new Exception("Lesson not found");

        var duration = GetAudioDuration(cmd.AudioStream, cmd.FileName);

        var storedFileName = await _fileStorage.SaveFileAsync(
            cmd.AudioStream,
            cmd.FileName,
            cmd.ContentType
        );

        var audio = new AudioResource(
            storedFileName,
            cmd.ContentType,
            cmd.Length
        );

        var exercise = new Domain.Entities.Exercise(
            cmd.LessonId,
            cmd.Title,
            audio,
            cmd.Difficulty,
            duration
        );

        if (cmd.SubtitleStream != null && !string.IsNullOrWhiteSpace(cmd.SubtitleFileName))
        {
            // 修改 1：先把 srt 字幕文件保存到 uploads/subtitles
            await _fileStorage.SaveFileAsync(
                cmd.SubtitleStream,
                cmd.SubtitleFileName,
                "text/plain"
            );

            // 修改 2：保存文件后，重置流位置，否则下面解析字幕可能读不到内容
            if (cmd.SubtitleStream.CanSeek)
            {
                cmd.SubtitleStream.Position = 0;
            }

            // 修改 3：继续解析 srt 内容，并保存到数据库 SubtitleSegments
            var segments = await _subtitleParser.ParseAsync(
                cmd.SubtitleStream,
                cmd.SubtitleFileName
            );

            foreach (var seg in segments.OrderBy(x => x.Sequence))
            {
                exercise.AddSubtitleSegment(
                    seg.Sequence,
                    seg.StartTime,
                    seg.EndTime,
                    seg.Text
                );
            }
        }

        await _lessonRepo.AddExerciseAsync(exercise);
        await _lessonRepo.SaveChangesAsync();

        return exercise.ToDto(_fileStorage.GetPublicUrl(""));
    }
    public async Task<ExerciseDto?> GetExerciseByIdAsync(Guid exerciseId)
    {
        var lesson = await _lessonRepo.FindByExerciseIdAsync(exerciseId);
        if (lesson == null)
            return null;

        var exercise = lesson.Exercises.FirstOrDefault(e => e.Id == exerciseId);
        if (exercise == null)
            return null;

        return MapToDto(exercise);
    }

    public async Task<bool> DeleteExerciseAsync(Guid exerciseId)
    {
        var lesson = await _lessonRepo.FindByExerciseIdAsync(exerciseId);
        if (lesson == null)
            return false;

        var exercise = lesson.Exercises.First(e => e.Id == exerciseId);

        await _fileStorage.DeleteFileAsync(exercise.Audio.FileName);

        lesson.RemoveExercise(exerciseId);

        await _lessonRepo.SaveChangesAsync();
        return true;
    }

public async Task UpdateExerciseAsync(UpdateExerciseCommand cmd)
{
    var exercise = await _lessonRepo.GetExerciseForUpdateAsync(cmd.Id)
                   ?? throw new Exception("Exercise not found");

    exercise.UpdateBasicInfo(cmd.Title, cmd.Difficulty);

    if (cmd.NewAudioStream != null &&
        !string.IsNullOrWhiteSpace(cmd.FileName) &&
        !string.IsNullOrWhiteSpace(cmd.ContentType))
    {
        var newDuration = GetAudioDuration(cmd.NewAudioStream, cmd.FileName);

        if (newDuration <= TimeSpan.Zero)
        {
            throw new Exception("Could not detect audio duration.");
        }

        if (cmd.NewAudioStream.CanSeek)
        {
            cmd.NewAudioStream.Position = 0;
        }

        await _fileStorage.DeleteFileAsync(exercise.Audio.FileName);

        var newFileName = await _fileStorage.SaveFileAsync(
            cmd.NewAudioStream,
            cmd.FileName,
            cmd.ContentType
        );

        var newAudioSize = cmd.NewAudioStream.CanSeek
            ? cmd.NewAudioStream.Length
            : 1;

        var newAudio = new AudioResource(
            newFileName,
            cmd.ContentType,
            newAudioSize
        );

        exercise.ChangeAudio(newAudio, newDuration);
    }

    if (cmd.SubtitleStream != null &&
        !string.IsNullOrWhiteSpace(cmd.SubtitleFileName))
    {
        await _fileStorage.SaveFileAsync(
            cmd.SubtitleStream,
            cmd.SubtitleFileName,
            "text/plain"
        );

        if (cmd.SubtitleStream.CanSeek)
        {
            cmd.SubtitleStream.Position = 0;
        }

        var parsedSegments = await _subtitleParser.ParseAsync(
            cmd.SubtitleStream,
            cmd.SubtitleFileName
        );

        var newSubtitleSegments = parsedSegments
            .OrderBy(x => x.Sequence)
            .Select(x => new SubtitleSegment(
                exercise.Id,
                x.Sequence,
                x.StartTime,
                x.EndTime,
                x.Text
            ))
            .ToList();

        await _lessonRepo.ReplaceSubtitleSegmentsAsync(
            exercise.Id,
            newSubtitleSegments
        );
    }

    await _lessonRepo.SaveChangesAsync();
}
    
    private ExerciseDto MapToDto(Domain.Entities.Exercise exercise)
    {
        return new ExerciseDto
        {
            Id = exercise.Id,
            LessonId = exercise.LessonId,
            Title = exercise.Title,
            AudioUrl = exercise.Audio.GetUrl(_fileStorage.GetPublicUrl("")),
            Transcript = exercise.Transcript,
            Difficulty = (int)exercise.Difficulty,
            DurationSeconds = exercise.DurationSeconds,
            Subtitles = exercise.SubtitleSegments
                .OrderBy(s => s.Sequence)
                .Select(s => new SubtitleSegmentDto
                {
                    Sequence = s.Sequence,
                    StartSeconds = s.StartTime.TotalSeconds,
                    EndSeconds = s.EndTime.TotalSeconds,
                    Text = s.Text
                })
                .ToList()
        };
    }

    private static TimeSpan GetAudioDuration(Stream stream, string fileName)
    {
        if (!stream.CanSeek)
            throw new InvalidOperationException("Stream must support seeking.");

        var tempPath = Path.Combine(
            Path.GetTempPath(),
            $"{Guid.NewGuid()}{Path.GetExtension(fileName)}"
        );

        try
        {
            stream.Position = 0;

            using (var fs = File.Create(tempPath))
            {
                stream.CopyTo(fs);
            }

            using var tagFile = TagLib.File.Create(tempPath);
            return tagFile.Properties.Duration;
        }
        finally
        {
            if (stream.CanSeek)
                stream.Position = 0;

            if (File.Exists(tempPath))
                File.Delete(tempPath);
        }
    }

}