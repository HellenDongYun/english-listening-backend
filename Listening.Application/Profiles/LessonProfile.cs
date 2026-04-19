using AutoMapper;
using Listening.Application.Dtos;
using Listening.Application.Exercise.Commands;
using Listening.Domain.Entities;

namespace Listening.Application.Profiles;

public class LessonProfile : Profile
{
    public LessonProfile()
    {
        CreateMap<AudioResource, AudioResourceDto>()
            .ForMember(dest => dest.Url,
                opt => opt.MapFrom(src => $"https://your-storage.com/{src.FileName}"));

        // ===== Lesson -> LessonDto =====
        CreateMap<Lesson, LessonDto>()
            .ForMember(dest => dest.Exercises,
                opt => opt.MapFrom(src => src.Exercises))
            .ForMember(dest => dest.ImagePath,
                opt => opt.MapFrom(src => src.ImagePath));

        CreateMap<Listening.Domain.Entities.Exercise, ExerciseDto>()
            .ForMember(dest => dest.Id,
                opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.LessonId,
                opt => opt.MapFrom(src => src.LessonId))
            .ForMember(dest => dest.Title,
                opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Transcript,
                opt => opt.MapFrom(src => src.Transcript))
            .ForMember(dest => dest.Difficulty,
                opt => opt.MapFrom(src => (int)src.Difficulty))
            .ForMember(dest => dest.DurationSeconds,
                opt => opt.MapFrom(src => src.DurationSeconds))
            .ForMember(dest => dest.AudioUrl,
                opt => opt.MapFrom(src => src.Audio.GetUrl("/uploads")));

        // ===== 删除 CreateLessonRequest -> Lesson 的自动构造 =====
        // 因为 imagePath 需要在 Controller 保存文件后手动传入
    }
}