using Application.Interfaces;
using Domain.Entities;
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
        return await _context.Courses
            .Include(c => c.Lessons)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<(IEnumerable<Course> Items, int TotalCount)> GetAllAsync(int page, int pageSize)
    {
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
        var query = _context.Courses.Include(c => c.Lessons).AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(c => c.Title.Contains(q));
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
        await _context.Courses.AddAsync(course);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Course course)
    {
        _context.Courses.Update(course);
        await _context.SaveChangesAsync();
    }
}
