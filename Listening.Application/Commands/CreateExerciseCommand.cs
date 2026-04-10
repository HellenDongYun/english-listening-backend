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

    // 建议增加以下字段，以匹配 Exercise 实体的构造需求
    public string Transcript { get; init; } = null!;
    
    // 由前端或上传组件预先计算好的时长（秒）
    public double DurationSeconds { get; init; } 
    
    // 难度等级 (1: Easy, 2: Medium, 3: Hard)
    public int Difficulty { get; init; } 
}