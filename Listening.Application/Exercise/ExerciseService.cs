using Listening.Application.Dtos;
using Listening.Application.Exercise.Commands;
using Listening.Application.Interfaces;
using Listening.Domain.Entities;

namespace Listening.Application.Exercise;

public class ExerciseService
{
    private readonly ILessonRepository _lessonRepo;
    private readonly IFileStorage _fileStorage;
    private readonly ISubtitleParser _subtitleParser;

    public ExerciseService(
        ILessonRepository lessonRepo,
        IFileStorage fileStorage)
    {
        _lessonRepo = lessonRepo;
        _fileStorage = fileStorage;
    }

    public async Task<ExerciseDto> CreateExerciseAsync(CreateExerciseCommand cmd)
    {
        var lesson = await _lessonRepo.GetByIdAsync(cmd.LessonId)
                     ?? throw new Exception("Lesson not found");

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

        var duration = TimeSpan.FromSeconds(cmd.DurationSeconds);
        var difficulty = cmd.Difficulty;

        var exercise = new Domain.Entities.Exercise(
            cmd.LessonId,
            cmd.Title,
            audio,
            difficulty,
            duration
        );

        // 解析 subtitle 文件并写入 Exercise
        if (cmd.SubtitleStream != null && !string.IsNullOrWhiteSpace(cmd.SubtitleFileName))
        {
            var segments = await _subtitleParser.ParseAsync(cmd.SubtitleStream, cmd.SubtitleFileName);

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

        lesson.AddExercise(exercise);

        await _lessonRepo.SaveChangesAsync();

        return new ExerciseDto
        {
            Id = exercise.Id,
            LessonId = lesson.Id,
            Title = exercise.Title,
            AudioUrl = audio.GetUrl(_fileStorage.GetPublicUrl("")),
            Transcript = exercise.Transcript
        };
    }

    public async Task<ExerciseDto?> GetExerciseByIdAsync(Guid exerciseId)
    {
        // 1️⃣ 通过 ExerciseId 找到所属 Lesson（聚合根）
        var lesson = await _lessonRepo.FindByExerciseIdAsync(exerciseId);
        if (lesson == null)
            return null;

        // 2️⃣ 从聚合中取 Exercise
        var exercise = lesson.Exercises.FirstOrDefault(e => e.Id == exerciseId);
        if (exercise == null)
            return null;

        // 3️⃣ 映射为 DTO（Application 层职责）
        return new ExerciseDto
        {
            Id = exercise.Id,
            LessonId = lesson.Id,
            Title = exercise.Title,
            Transcript = exercise.Transcript,
            AudioUrl = exercise.Audio.GetUrl(
                _fileStorage.GetPublicUrl("")
            )
        };
    }

    public async Task<bool> DeleteExerciseAsync(Guid exerciseId)
    {
        //1. find exercise and its parent lesson
        //in DDD, we usually find the aggregate root that contains the entity
        var lesson = await _lessonRepo.FindByExerciseIdAsync(exerciseId);
        if (lesson == null)
            return false;
        var exercise = lesson.Exercises.First(e => e.Id == exerciseId);
        //2.remove physical file
        await _fileStorage.DeleteFileAsync(exercise.Audio.FileName);
        //3. remove from aggregate root
        lesson.RemoveExercise(exerciseId);
        //4. persist
        await _lessonRepo.SaveChangesAsync();
        return true;

    }

    public async Task UpdateExerciseAsync(UpdateExerciseCommand cmd)
    {
        var lesson = await _lessonRepo.FindByExerciseIdAsync(cmd.Id)
                     ?? throw new Exception("Lesson not found");

        var exercise = lesson.Exercises.FirstOrDefault(e => e.Id == cmd.Id);
        if (exercise == null)
            throw new Exception("Exercise not found");

        exercise.UpdateBasicInfo(
            cmd.Title,
            (DifficultyLevel)cmd.Difficulty
        );

        if (cmd.NewAudioStream != null)
        {
            await _fileStorage.DeleteFileAsync(exercise.Audio.FileName);

            var newFileName = await _fileStorage.SaveFileAsync(
                cmd.NewAudioStream,
                cmd.FileName,
                cmd.ContentType
            );

            var newAudio = new AudioResource(
                newFileName,
                cmd.ContentType,
                cmd.Length > 0 ? cmd.Length : 1
            );

            exercise.ChangeAudio(newAudio, cmd.Duration);
        }

        await _lessonRepo.SaveChangesAsync();
    }
}