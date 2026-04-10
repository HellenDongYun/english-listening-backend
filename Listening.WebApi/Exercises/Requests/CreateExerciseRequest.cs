using Listening.Domain.Entities;

namespace Listening.Request;

public class CreateExerciseRequest
{
    public string Title { get; set; } = null!;
    public IFormFile AudioFile { get; set; } = null!;
    public string Transcript { get; set; } = null!;
    public DifficultyLevel Difficulty { get; set; }
    public double DurationSeconds { get; set; }
    public string FileName { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public long Size { get; set; }
}