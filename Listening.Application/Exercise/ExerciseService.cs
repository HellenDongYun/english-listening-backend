using Listening.Application.Dtos;
using Listening.Application.Exercise.Commands;
using Listening.Domain.Entities;

namespace Listening.Application.Exercise;

public class ExerciseService
{
    private readonly ILessonRepository _lessonRepo;
    private readonly IFileStorage _fileStorage;

    public ExerciseService(
        ILessonRepository lessonRepo,
        IFileStorage fileStorage)
    {
        _lessonRepo = lessonRepo;
        _fileStorage = fileStorage;
    }

    public async Task<ExerciseDto> CreateExerciseAsync(CreateExerciseCommand cmd)
    {
        // 1️⃣ 查找聚合根 Lesson
        var lesson = await _lessonRepo.GetByIdAsync(cmd.LessonId)?? throw new Exception("Lesson not found");

        // 2️⃣ 保存音频文件（Application 调用 Infrastructure）
        var storedFileName = await _fileStorage.SaveFileAsync(
            cmd.AudioStream,
            cmd.FileName,
            cmd.ContentType
        );

        // 3️⃣ 创建值对象 AudioResource
        var audio = new AudioResource(
            storedFileName,
            cmd.ContentType,
            cmd.Length
        );
        // 2. 处理时长转换
        var duration = TimeSpan.FromSeconds(cmd.DurationSeconds);
        // 3. 处理枚举转换（确保类型匹配）
        // 假设领域层枚举为 DifficultyLevel
        var difficulty = (DifficultyLevel)cmd.Difficulty;

        // 4️⃣ 创建领域实体 Exercise
        var exercise = new Domain.Entities.Exercise(
            cmd.LessonId, 
            cmd.Title, 
            audio, 
            cmd.Transcript,
            difficulty,    // 修复：添加缺少的难度参数
            duration       // 修复：添加缺少的时长参数
        );

        // 5️⃣ 通过聚合根维护一致性
        lesson.AddExercise(exercise);

        // 6️⃣ 提交事务
        await _lessonRepo.SaveChangesAsync();

        // 7️⃣ 返回 DTO（给 Web 层 / 前端）
        return new ExerciseDto
        {
            Id = exercise.Id,
            LessonId = lesson.Id,
            Title = exercise.Title,
            AudioUrl = audio.GetUrl(_fileStorage.GetPublicUrl("")),
            Transcript = exercise.Transcript
        };
    }
    
    public async Task<ExerciseDto?> GetExerciseByIdAsync(Guid exerciseId)
    {
        // 1️⃣ 通过 ExerciseId 找到所属 Lesson（聚合根）
        var lesson = await _lessonRepo.FindByExerciseIdAsync(exerciseId);
        if (lesson == null)
            return null;

        // 2️⃣ 从聚合中取 Exercise
        var exercise = lesson.Exercises.FirstOrDefault(e => e.Id == exerciseId);
        if (exercise == null)
            return null;

        // 3️⃣ 映射为 DTO（Application 层职责）
        return new ExerciseDto
        {
            Id = exercise.Id,
            LessonId = lesson.Id,
            Title = exercise.Title,
            Transcript = exercise.Transcript,
            AudioUrl = exercise.Audio.GetUrl(
                _fileStorage.GetPublicUrl("")
            )
        };
    }

    public async Task<bool> DeleteExerciseAsync(Guid exerciseId)
    {
        //1. find exercise and its parent lesson
        //in DDD, we usually find the aggregate root that contains the entity
        var lesson = await _lessonRepo.FindByExerciseIdAsync(exerciseId);
        if (lesson == null)
            return false;
        var exercise = lesson.Exercises.First(e=>e.Id==exerciseId);
        //2.remove physical file
        await _fileStorage.DeleteFileAsync(exercise.Audio.FileName);
        //3. remove from aggregate root
        lesson.RemoveExercise(exerciseId);
        //4. persist
        await _lessonRepo.SaveChangesAsync();
        return true;

    }

    public async Task UpdateExerciseAsync(UpdateExerciseCommand cmd)
    {
        // 1. 获取包含 Exercise 的聚合根
        var lesson = await _lessonRepo.FindByExerciseIdAsync(cmd.Id)?? throw new Exception("Lesson not found");
        var exercise = lesson.Exercises.FirstOrDefault(e=>e.Id==cmd.Id);
        if (exercise == null) throw new Exception("Exercise not found");
        exercise.UpdateBasicInfo(cmd.Title, cmd.Transcript,cmd.Difficulty);
        if (cmd.NewAudioStream != null)
        {
            await _fileStorage.DeleteFileAsync(exercise.Audio.FileName);
            var newFileName = await _fileStorage.SaveFileAsync(cmd.NewAudioStream,cmd.FileName,cmd.ContentType);
            var newAudio = new AudioResource(
                newFileName, cmd.ContentType, 0);
            exercise.ChangeAudio(newAudio,cmd.Duration);
        }
        // 4. 持久化
        await _lessonRepo.SaveChangesAsync();

    }
}