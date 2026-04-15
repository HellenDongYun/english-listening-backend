using Listening.Domain.Entities;

namespace Listening.Application.Dtos;

public class ExerciseDto
{
    public Guid Id { get; set; }

    public Guid LessonId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string AudioUrl { get; set; } = string.Empty;

    public string Transcript { get; set; } = string.Empty;

    public int Difficulty { get; set; }

    public double DurationSeconds { get; set; }

    public List<SubtitleSegmentDto> Subtitles { get; set; } = new();
} ;