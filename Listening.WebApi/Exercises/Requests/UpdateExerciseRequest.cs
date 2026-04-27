
namespace Listening.Application.Dtos;

public class UpdateExerciseRequest
{
    public string Title { get; set; }
    public IFormFile? AudioFile { get; set; } // 可选，如果不上传则不更新音频
    // 字幕文件（srt）
    public IFormFile? SubtitleFile { get; set; }

    // 难度
    public int Difficulty { get; set; }
}