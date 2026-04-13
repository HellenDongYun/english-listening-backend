namespace Listening.Application.Dtos;

public class SubtitleParseResult
{
    public int Sequence { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string Text { get; set; } = string.Empty;
}