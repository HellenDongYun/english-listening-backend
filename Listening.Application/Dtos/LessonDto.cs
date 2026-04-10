namespace Listening.Application.Dtos;

public class LessonDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public List<ExerciseDto> Exercises { get; set; } = new();
    
}