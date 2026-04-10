namespace Listening.Application.Dtos;

public class ExerciseCreateDto
{
    public string Title { get; set; }
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public long Size { get; set; }
    public string Transcript { get; set; }
    public int Difficulty { get; set; }
    public double DurationSeconds { get; set; }
}