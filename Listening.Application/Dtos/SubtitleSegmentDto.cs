namespace Listening.Application.Dtos;

public class SubtitleSegmentDto
{
    public int Sequence { get; set; }
    public double StartSeconds { get; set; }
    public double EndSeconds { get; set; }
    public string Text { get; set; } = string.Empty;
}