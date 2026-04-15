namespace Listening.Domain.Entities;

public class Exercise
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public Guid LessonId { get; private set; }

    public AudioResource Audio { get; private set; } = default!;
    public DifficultyLevel Difficulty { get; private set; }

    private TimeSpan _duration;
    public TimeSpan Duration => _duration;
    public double DurationSeconds => _duration.TotalSeconds;

    // 全文 transcript，作为聚合内的派生结果
    private string _transcript = string.Empty;
    public string Transcript => _transcript;

    private readonly List<SubtitleSegment> _subtitleSegments = new();
    public IReadOnlyCollection<SubtitleSegment> SubtitleSegments => _subtitleSegments.AsReadOnly();

    private Exercise()
    {
        // EF Core
    }

    public Exercise(
        Guid lessonId,
        string title,
        AudioResource audio,
        DifficultyLevel difficulty,
        TimeSpan duration)
    {
        if (lessonId == Guid.Empty)
            throw new ArgumentException("LessonId cannot be empty.", nameof(lessonId));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty.", nameof(title));

        if (audio is null)
            throw new ArgumentNullException(nameof(audio));

        if (duration <= TimeSpan.Zero)
            throw new ArgumentException("Duration must be greater than zero.", nameof(duration));

        Id = Guid.NewGuid();
        LessonId = lessonId;
        Title = title.Trim();
        Audio = audio;
        Difficulty = difficulty;
        _duration = duration;
        _transcript = string.Empty;
    }

    public void UpdateBasicInfo(string title, DifficultyLevel difficulty)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("标题不能为空", nameof(title));

        Title = title.Trim();
        Difficulty = difficulty;
    }

    public void ChangeAudio(AudioResource audio, TimeSpan newDuration)
    {
        if (audio is null)
            throw new ArgumentNullException(nameof(audio));

        if (newDuration <= TimeSpan.Zero)
            throw new ArgumentException("Duration must be greater than zero.", nameof(newDuration));

        Audio = audio;
        _duration = newDuration;

        ClearSubtitles();
    }
    private void ClearSubtitles()
    {
        _subtitleSegments.Clear();
        _transcript = string.Empty;
    }

    public void ReplaceSubtitleSegments(IEnumerable<SubtitleSegment> segments)
    {
        if (segments is null)
            throw new ArgumentNullException(nameof(segments));
        

        var orderedSegments = segments
            .OrderBy(s => s.Sequence)
            .ToList();

        ValidateSegments(orderedSegments);

        _subtitleSegments.Clear();
        _subtitleSegments.AddRange(orderedSegments);

        RebuildTranscript();
    }

    public SubtitleSegment AddSubtitleSegment(
        int sequence,
        TimeSpan startTime,
        TimeSpan endTime,
        string text)
    {
        var segment = new SubtitleSegment(Id, sequence, startTime, endTime, text);

        _subtitleSegments.Add(segment);
        NormalizeSequence();
        ValidateSegments(_subtitleSegments.OrderBy(x => x.Sequence).ToList());
        RebuildTranscript();

        return segment;
    }

    public void UpdateSubtitleSegment(
        Guid segmentId,
        TimeSpan startTime,
        TimeSpan endTime,
        string text)
    {
        var segment = _subtitleSegments.FirstOrDefault(x => x.Id == segmentId);
        if (segment is null)
            throw new InvalidOperationException("Subtitle segment not found.");

        segment.UpdateTiming(startTime, endTime);
        segment.UpdateText(text);

        ValidateSegments(_subtitleSegments.OrderBy(x => x.Sequence).ToList());
        RebuildTranscript();
    }

    public void RemoveSubtitleSegment(Guid segmentId)
    {
        var segment = _subtitleSegments.FirstOrDefault(x => x.Id == segmentId);
        if (segment is null)
            throw new InvalidOperationException("Subtitle segment not found.");

        _subtitleSegments.Remove(segment);
        NormalizeSequence();
        RebuildTranscript();
    }

    public void ReorderSubtitleSegment(Guid segmentId, int newSequence)
    {
        if (newSequence < 1)
            throw new ArgumentException("Sequence must be greater than 0.", nameof(newSequence));

        var segment = _subtitleSegments.FirstOrDefault(x => x.Id == segmentId);
        if (segment is null)
            throw new InvalidOperationException("Subtitle segment not found.");

        segment.UpdateSequence(newSequence);

        NormalizeSequence();
        ValidateSegments(_subtitleSegments.OrderBy(x => x.Sequence).ToList());
        RebuildTranscript();
    }

    private void RebuildTranscript()
    {
        _transcript = string.Join(" ", _subtitleSegments
            .OrderBy(x => x.Sequence)
            .Select(x => x.Text.Trim()));
    }

    private void NormalizeSequence()
    {
        var ordered = _subtitleSegments
            .OrderBy(x => x.Sequence)
            .ThenBy(x => x.StartTime)
            .ToList();

        for (int i = 0; i < ordered.Count; i++)
        {
            ordered[i].UpdateSequence(i + 1);
        }
    }

    private static void ValidateSegments(IReadOnlyList<SubtitleSegment> segments)
    {
        for (int i = 0; i < segments.Count; i++)
        {
            var current = segments[i];

            if (current.StartTime < TimeSpan.Zero)
                throw new InvalidOperationException("Subtitle start time cannot be negative.");

            if (current.EndTime <= current.StartTime)
                throw new InvalidOperationException("Subtitle end time must be greater than start time.");

            if (i > 0)
            {
                var previous = segments[i - 1];

                if (current.Sequence <= previous.Sequence)
                    throw new InvalidOperationException("Subtitle sequence must be strictly increasing.");

                // 如果你不允许时间重叠，就保留这个校验
                if (current.StartTime < previous.EndTime)
                    throw new InvalidOperationException("Subtitle segments cannot overlap.");
            }
        }
    }
}