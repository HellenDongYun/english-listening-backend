using Listening.Domain.Entities;

namespace Listening.Application.Exercise.Commands;

public class UpdateExerciseCommand
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Transcript { get; set; } = string.Empty;
    public DifficultyLevel Difficulty { get; set; }
    // 音频
    public Stream? NewAudioStream { get; set; }
    public string? FileName { get; set; }
    public string? ContentType { get; set; }
    public long Length { get; set; }
    public TimeSpan Duration { get; set; }
    // ：字幕（
    public Stream? SubtitleStream { get; set; }

    public string? SubtitleFileName { get; set; }
}