namespace Listening.Domain.Entities;

public interface ILessonRepository
{
    Task<IEnumerable<Lesson>> GetAllAsync();
    Task<Lesson?> GetByIdAsync(Guid lessonId);
    Task<Lesson?> FindByExerciseIdAsync(Guid exerciseId);
    Task AddAsync(Lesson lesson);
    Task SaveChangesAsync();
    // 分页，列表
    Task<bool> DeleteAsync(Guid lessonId);
    Task UpdateAsync(Lesson lesson);
    Task<bool> ExistsAsync(Guid lessonId);
    Task AddExerciseAsync(Exercise exercise);
    Task<Exercise?> GetExerciseForUpdateAsync(Guid exerciseId);
    Task ReplaceSubtitleSegmentsAsync(Guid exerciseId, IEnumerable<SubtitleSegment> segments);
}