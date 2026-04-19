using System.ComponentModel.DataAnnotations;

namespace Listening.Application.Dtos;

public class UpdateLessonDto
{
    [Required(ErrorMessage = "Class title is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "The title length must be between 3 and 100 characters")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Class description is required")]
    public string Description { get; set; } = string.Empty;

    // 如果允许修改内容、顺序等，在此处添加字段
    // 不要放 Id, CreateTime 等不该被用户修改的字段
    
    
    //public string? ImagePath { get; set; }
}