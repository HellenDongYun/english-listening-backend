using Listening.Domain.Entities;

namespace Listening.Application.Dtos;

public class ExerciseDto
{
    public Guid Id { get; set; }
    
    public Guid LessonId { get; set; }
    
    public string Title { get; set; } = null;
    
    public string AudioUrl { get; set; } = null;

    public string Transcript { get; set; } = null;
    public int Difficulty { get; set; }
    public double DurationSeconds { get; set; } // 转换为秒方便前端处理
} ;