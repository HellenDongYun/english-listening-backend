using System.ComponentModel.DataAnnotations;
using Listening.Application.Dtos;

namespace Listening.Application.Exercise.Commands;

public class CreateLessonRequest
{
    [Required]
    [StringLength(100)]
    public string Title { get; set; } = null!;

    public string? Description { get; set; }
    
   // public string? ImagePath { get; set; }
        
    // 如果创建课程时允许同时添加练习，可以保留此项
    // public List<ExerciseCreateDto> Exercises { get; set; } = new();
    
}