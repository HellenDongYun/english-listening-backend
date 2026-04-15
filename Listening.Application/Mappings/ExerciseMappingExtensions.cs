using Listening.Application.Dtos;
namespace Listening.Application.Mappings;

public static class ExerciseMappingExtensions
{
    public static ExerciseDto ToDto(this Domain.Entities.Exercise exercise, string baseUrl)
    {
        return new ExerciseDto
        {
            Id = exercise.Id,
            LessonId = exercise.LessonId,
            Title = exercise.Title,
            AudioUrl = exercise.Audio.GetUrl(baseUrl),
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
    
}