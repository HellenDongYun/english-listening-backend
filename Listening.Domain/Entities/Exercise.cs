namespace Listening.Domain.Entities;

public class Exercise
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public Guid LessonId { get; private set; }
    // 导航属性：在 EF Core 中建议将其设置为可空，或通过构造函数注入
    public AudioResource Audio { get; private set; }
    // new add property
    public DifficultyLevel Difficulty { get; private set; }
    // 使用字段存储，方便 EF Core 映射私有成员
    private TimeSpan _duration;
    public TimeSpan Duration =>_duration;
    // 新增一个只读属性：给外界用秒
    public double DurationSeconds => _duration.TotalSeconds;
    private string _transcript;
    public string Transcript => _transcript;
    

    private Exercise() //ef 
    {
    }

    public Exercise(Guid lessonId,string title, AudioResource audio, string transcript,DifficultyLevel difficulty, TimeSpan duration)
    {
        Id = Guid.NewGuid();
        LessonId = lessonId;
        Title = string.IsNullOrWhiteSpace(title) ? throw new ArgumentException("Title can not be null") : title;
        Audio = audio?? throw new ArgumentNullException(nameof(audio));
        _transcript = transcript;
        Difficulty = difficulty;
        _duration = duration;
    }
    // 领域方法：修改基本信息
    public void UpdateBasicInfo(string title, string transcript, DifficultyLevel difficulty)
    {
        
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("标题不能为空");
        if (string.IsNullOrWhiteSpace(transcript)) throw new ArgumentException("文本内容不能为空");
        
        Title = title;
        _transcript = transcript;
        Difficulty = difficulty;
    }

    // 领域方法：更换音频
    public void ChangeAudio(AudioResource audio,TimeSpan newDuration)
    {
        Audio = audio ?? throw new ArgumentNullException(nameof(audio));
        _duration = newDuration;
    }
}