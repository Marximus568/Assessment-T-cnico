using Domain.Entities;

namespace Application.Interfaces;

public interface ICourseRepository
{
    Task<Course?> GetByIdAsync(Guid id);
    Task<(IEnumerable<Course> Items, int TotalCount)> GetAllAsync(int page, int pageSize);
    Task<(IEnumerable<Course> Items, int TotalCount)> SearchAsync(string q, string status, int page, int pageSize);
    Task AddAsync(Course course);
    Task UpdateAsync(Course course);
    Task AddLessonAsync(Guid courseId, Lesson lesson);
}