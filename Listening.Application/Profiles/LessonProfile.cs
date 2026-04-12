using Listening.Application.Dtos;
using Listening.Domain.Entities;
using AutoMapper;
using Listening.Application.Exercise.Commands;


namespace Listening.Application.Profiles;
public class LessonProfile:Profile
{
    public LessonProfile()
    {
        // 映射 AudioResource -> AudioResourceDto
        CreateMap<AudioResource, AudioResourceDto>()
            // 建议：URL 拼接通常建议在领域层或服务层处理，
            // 但如果在这里处理，请确保路径逻辑正确
            .ForMember(dest => dest.Url, opt => opt.MapFrom(src => $"https://your-storage.com/{src.FileName}"));
        // --- 1. 从实体(Entity) 映射到 传输对象(Dto) [用于查询接口] ---
 
        // 映射 Lesson -> LessonDto
        CreateMap<Lesson, LessonDto>()
            .ForMember(dest => dest.Exercises, opt => opt.MapFrom(src => src.Exercises));

        // 配置 Exercise -> ExerciseDto 的映射
        CreateMap<Domain.Entities.Exercise, ExerciseDto>()
            // 映射基础字段
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.LessonId, opt => opt.MapFrom(src => src.LessonId))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Transcript, opt => opt.MapFrom(src => src.Transcript))
            .ForMember(dest => dest.Difficulty, opt => opt.MapFrom(src => src.Difficulty))

            // TimeSpan → double（秒）
            .ForMember(dest => dest.DurationSeconds, 
                opt => opt.MapFrom(src => src.DurationSeconds))

            // 映射 AudioResource → AudioResourceDto
            .ForMember(dest => dest.Audio, opt => opt.MapFrom(src => src.Audio))

            // AudioUrl（如果你有文件访问 API）
            .ForMember(dest => dest.AudioUrl, 
                opt => opt.MapFrom(src => 
                    $"/api/audio/{src.Audio.Id}"));


 


        // --- 2. 从 创建请求(Request/Dto) 映射到 实体(Entity) [用于写入接口] ---

        // 映射 CreateLessonRequest -> Lesson
        // 使用 ConstructUsing 确保遵循领域驱动设计(DDD)的构造函数约束
        CreateMap<CreateLessonRequest, Lesson>()
            .ConstructUsing(src => new Lesson(src.Title, src.Description))
            // 忽略非构造函数参数的自动映射，防止报错
            .ForAllMembers(opt => opt.Ignore());

        // 映射 ExerciseCreateDto -> Exercise
        // 针对 TimeSpan 报错的重灾区，我们完全手动接管构造过程
        CreateMap<ExerciseCreateDto, Domain.Entities.Exercise>()
            .ConstructUsing((src, ctx) =>
            {
                var audio = new AudioResource(
                    src.FileName,
                    src.ContentType,
                    src.Size
                );

                var duration = TimeSpan.FromSeconds(src.DurationSeconds);
                var difficulty = (DifficultyLevel)src.Difficulty;

                return new Domain.Entities.Exercise(
                    Guid.Empty,
                    src.Title,
                    audio,
                    difficulty,
                    duration
                );
            })
            // 必须调用 Ignore，因为我们已经在 ConstructUsing 中处理了所有逻辑
            // 这可以防止 AutoMapper 再次尝试反射映射属性，从而避开报错
            .ForAllMembers(opt => opt.Ignore());
    }

}