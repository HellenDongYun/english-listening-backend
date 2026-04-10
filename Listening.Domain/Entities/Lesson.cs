namespace Listening.Domain.Entities;

public class Lesson:IAggregateRoot
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    
    //value object or navigator
    private readonly List<Exercise> _exercises = [];
    public IReadOnlyCollection<Exercise> Exercises => _exercises.AsReadOnly();

    public Lesson() { }  //for ef core
    public Lesson(string title, string? description)
    {
        Id = Guid.NewGuid();
        Title = title;
        Description = description;
    }
    public void AddExercise(Exercise exercise) => _exercises.Add(exercise);

    public void RemoveExercise(Guid exerciseId)
    {
        var exercise = _exercises.SingleOrDefault(x => x.Id == exerciseId);
        if (exercise != null)
            _exercises.Remove(exercise);
    }
    // 提供一个公共方法用于更新title description
    public void Update(string title, string description)
    {
        if (string.IsNullOrWhiteSpace(title)) 
            throw new ArgumentException("Title cannot be null or empty", nameof(title));

        this.Title = title;
        this.Description = description;
    }

}