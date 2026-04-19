namespace Listening.Domain.Entities;

public class Lesson : IAggregateRoot
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string? Description { get; private set; }

    // ===== 保存图片路径 =====
    public string? ImagePath { get; private set; }

    private readonly List<Exercise> _exercises = [];
    public IReadOnlyCollection<Exercise> Exercises => _exercises.AsReadOnly();

    public Lesson() { }  // for ef core

    public Lesson(string title, string? description, string? imagePath = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be null or empty", nameof(title));

        Id = Guid.NewGuid();
        Title = title;
        Description = description;
        ImagePath = imagePath;
    }

    public void AddExercise(Exercise exercise) => _exercises.Add(exercise);

    public void RemoveExercise(Guid exerciseId)
    {
        var exercise = _exercises.SingleOrDefault(x => x.Id == exerciseId);
        if (exercise != null)
            _exercises.Remove(exercise);
    }

    // =====Update 支持 imagePath =====
    public void Update(string title, string? description, string? imagePath)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be null or empty", nameof(title));

        Title = title;
        Description = description;
        ImagePath = imagePath;
    }

    public void UpdateImage(string? imagePath)
    {
        ImagePath = imagePath;
    }

    public void RemoveImage()
    {
        ImagePath = null;
    }
}