using Listening.Domain.Entities;

namespace Listening.Request;

public class CreateExerciseRequest
{
    public string Title { get; set; } = null!;
    public IFormFile AudioFile { get; set; } = null!;
    public DifficultyLevel Difficulty { get; set; }
    public IFormFile? SubtitleFile { get; set; }
}