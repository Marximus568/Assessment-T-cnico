using Domain.Entities;

namespace Application.Interfaces;

public interface ICourseRepository
{
    Task<Course?> GetByIdAsync(Guid id);
    Task AddAsync(Course course);
    Task UpdateAsync(Course course);
}