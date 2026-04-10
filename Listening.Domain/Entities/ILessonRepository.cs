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

}