
namespace Listening.Application.Dtos;

public class UpdateExerciseRequest
{
    public string Title { get; set; }
    public string Transcript { get; set; }
    public IFormFile? AudioFile { get; set; } // 可选，如果不上传则不更新音频
}