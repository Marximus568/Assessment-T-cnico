using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Config;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly AppDbContext _context;

    public CourseRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Course?> GetByIdAsync(Guid id)
    {
        // Global query filter automatically excludes IsDeleted = true
        return await _context.Courses
            .Include(c => c.Lessons)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<(IEnumerable<Course> Items, int TotalCount)> GetAllAsync(int page, int pageSize)
    {
        // Global query filter automatically applied to all queries
        var query = _context.Courses.Include(c => c.Lessons).AsQueryable();
        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return (items, totalCount);
    }

    public async Task<(IEnumerable<Course> Items, int TotalCount)> SearchAsync(string q, string status, int page, int pageSize)
    {
        // Global query filter automatically applied
        var query = _context.Courses.Include(c => c.Lessons).AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            // Case-insensitive search using EF.Functions.Like()
            // LIKE pattern: %query% matches anywhere in the title
            query = query.Where(c => EF.Functions.Like(c.Title, $"%{q}%"));
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            if (Enum.TryParse<Domain.Enums.CourseStatus>(status, true, out var courseStatus))
            {
                query = query.Where(c => c.Status == courseStatus);
            }
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return (items, totalCount);
    }

    public async Task AddAsync(Course course)
    {
        if (course == null)
            throw new ArgumentNullException(nameof(course));

        await _context.Courses.AddAsync(course);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Course course)
    {
        if (course == null)
            throw new ArgumentNullException(nameof(course));

        // Verify course exists (not soft-deleted)
        var existing = await _context.Courses.IgnoreQueryFilters().FirstOrDefaultAsync(c => c.Id == course.Id);
        if (existing == null)
            throw new KeyNotFoundException($"Course with ID {course.Id} not found");

        _context.Courses.Update(course);
        await _context.SaveChangesAsync();
    }

    public async Task AddLessonAsync(Guid courseId, Lesson lesson)
    {
        if (lesson == null)
            throw new ArgumentNullException(nameof(lesson));

        // Verify course exists
        var courseExists = await _context.Courses
            .AnyAsync(c => c.Id == courseId);

        if (!courseExists)
            throw new KeyNotFoundException($"Course with ID {courseId} not found");

        // Intelligent Reordering Logic: Shift subsequent lessons
        var subsequentLessons = await _context.Lessons
            .Where(l => l.CourseId == courseId && !l.IsDeleted && l.Order >= lesson.Order)
            .OrderBy(l => l.Order)
            .ToListAsync();

        foreach (var subsequentLesson in subsequentLessons)
        {
            subsequentLesson.UpdateOrder(subsequentLesson.Order + 1);
        }

        // Add the new lesson (CourseId is already set in constructor)
        await _context.Lessons.AddAsync(lesson);
        await _context.SaveChangesAsync();
    }
}
