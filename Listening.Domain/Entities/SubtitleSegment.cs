namespace Listening.Domain.Entities;

public class SubtitleSegment
{
    public Guid Id { get; private set; }
    public Guid ExerciseId { get; private set; }

    // 字幕顺序
    public int Sequence { get; private set; }

    // 开始和结束时间
    private TimeSpan _startTime;
    public TimeSpan StartTime => _startTime;

    private TimeSpan _endTime;
    public TimeSpan EndTime => _endTime;

    // 方便前端使用毫秒
    public double StartMilliseconds => _startTime.TotalMilliseconds;
    public double EndMilliseconds => _endTime.TotalMilliseconds;

    private string _text = string.Empty;
    public string Text => _text;

    private SubtitleSegment()
    {
        // EF Core
    }

    public SubtitleSegment(
        Guid exerciseId,
        int sequence,
        TimeSpan startTime,
        TimeSpan endTime,
        string text)
    {
        if (exerciseId == Guid.Empty)
            throw new ArgumentException("ExerciseId cannot be empty.", nameof(exerciseId));

        if (sequence < 1)
            throw new ArgumentException("Sequence must be greater than 0.", nameof(sequence));

        if (startTime < TimeSpan.Zero)
            throw new ArgumentException("Start time cannot be negative.", nameof(startTime));

        if (endTime <= startTime)
            throw new ArgumentException("End time must be greater than start time.", nameof(endTime));

        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Subtitle text cannot be empty.", nameof(text));

        Id = Guid.NewGuid();
        ExerciseId = exerciseId;
        Sequence = sequence;
        _startTime = startTime;
        _endTime = endTime;
        _text = text.Trim();
    }

    public void UpdateText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Subtitle text cannot be empty.", nameof(text));

        _text = text.Trim();
    }

    public void UpdateTiming(TimeSpan startTime, TimeSpan endTime)
    {
        if (startTime < TimeSpan.Zero)
            throw new ArgumentException("Start time cannot be negative.", nameof(startTime));

        if (endTime <= startTime)
            throw new ArgumentException("End time must be greater than start time.", nameof(endTime));

        _startTime = startTime;
        _endTime = endTime;
    }

    public void UpdateSequence(int sequence)
    {
        if (sequence < 1)
            throw new ArgumentException("Sequence must be greater than 0.", nameof(sequence));

        Sequence = sequence;
    }
}