using Listening.Domain.Entities;

namespace Listening.Application.Exercise.Commands;

public class UpdateExerciseCommand
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Transcript { get; set; } = string.Empty;

    /// <summary>
    /// 练习难度（例如：初级、中级、高级）
    /// </summary>
    public DifficultyLevel Difficulty { get; set; }

    /// <summary>
    /// 新音频流（如果需要更换音频则不为空）
    /// </summary>
    public Stream? NewAudioStream { get; set; }

    /// <summary>
    /// 音频文件名
    /// </summary>
    public string? FileName { get; set; }

    /// <summary>
    /// 音频 MIME 类型
    /// </summary>
    public string? ContentType { get; set; }

    /// <summary>
    /// 音频文件大小（字节）
    /// </summary>
    public long Length { get; set; }

    /// <summary>
    /// 音频时长（用于 ChangeAudio 方法）
    /// </summary>
    public TimeSpan Duration { get; set; }
}