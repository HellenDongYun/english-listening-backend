namespace Listening.Application.Dtos;

public class LessonCreateDto
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string? ImagePath { get; set; }
        
    // 如果允许创建课程时同时添加练习，可以加上这个属性
    public List<ExerciseCreateDto> Exercises { get; set; } = new();
    
}