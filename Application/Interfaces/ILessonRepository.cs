using Domain.Entities;

namespace Application.Interfaces;

public interface ILessonRepository
{
    Task AddAsync(Lesson lesson);
    Task<List<Lesson>> GetByCourseIdAsync(Guid courseId);
}