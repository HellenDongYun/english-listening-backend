using Listening.Domain.Entities;

namespace Listening.Application.Exercise.Commands;

public class CreateExerciseCommand
{
    public Guid LessonId { get; init; }
    public string Title { get; init; } = null!;
    
    // 物理文件处理
    public Stream AudioStream { get; init; } = null!;
    public string FileName { get; init; } = null!;
    public string ContentType { get; init; } = null!;
    public long Length { get; init; }
    
    // 难度等级 (1: Easy, 2: Medium, 3: Hard)
    public  DifficultyLevel Difficulty { get; init; } 
    //字幕
    public Stream? SubtitleStream { get; init; }
    public string? SubtitleFileName { get; init; }
}