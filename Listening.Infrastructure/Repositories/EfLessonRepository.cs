using Listening.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Listening.Infrastructure.Repositories;

public class EfLessonRepository:ILessonRepository
{
    private readonly AppDbContext _db;

    public EfLessonRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(Lesson lesson)
    {
        _db.Lessons.Add(lesson); 
        await _db.SaveChangesAsync(); 
        
    }

    public async Task SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
    }


    public async Task<Lesson?> GetByIdAsync(Guid lessonId)
    {
        return await _db.Lessons
            .Include(l => l.Exercises)
            .ThenInclude(e => e.Audio)
            .Include(l => l.Exercises)
            .ThenInclude(e => e.SubtitleSegments)
            .FirstOrDefaultAsync(l => l.Id == lessonId);
    }

    public async Task<Lesson?> FindByExerciseIdAsync(Guid exerciseId) => await _db.Lessons
        .Include(l => l.Exercises)
        .ThenInclude(e => e.Audio)
        .Include(l => l.Exercises)
        .ThenInclude(e => e.SubtitleSegments)
        .FirstOrDefaultAsync(l => l.Exercises.Any(e => e.Id == exerciseId));

    public async Task<IEnumerable<Lesson>> GetAllAsync()
    {
        return await _db.Lessons.Include(l=>l.Exercises).AsNoTracking().ToListAsync();
    }

    public async Task UpdateAsync(Lesson lesson)
    {
        _db.Lessons.Update(lesson);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Guid lessonId)
    {
        return await _db.Lessons.AnyAsync(l => l.Id == lessonId);
    }

    public async Task AddExerciseAsync(Exercise exercise)
    {
        await _db.Exercises.AddAsync(exercise);
    }

    public async Task<bool> DeleteAsync(Guid lessonId)
    {
        var lesson = await _db.Lessons.FirstOrDefaultAsync(l => l.Id == lessonId);
        if (lesson == null)
        {
            return false;
        }
        _db.Lessons.Remove(lesson);
        await _db.SaveChangesAsync();
        return true;
    }

}