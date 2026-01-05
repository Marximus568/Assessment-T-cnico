using Application.DTOs;

namespace Application.Interfaces;

public interface ILessonService
{
    Task<Guid> AddLessonAsync(Guid courseId, string title, int order);
    Task<LessonDto?> GetLessonAsync(Guid courseId, Guid lessonId);
    Task<PagedResult<LessonDto>> GetLessonsByCourseIdAsync(Guid courseId, int page, int pageSize);
    Task ReorderLessonAsync(Guid courseId, Guid lessonId, int newOrder);
    Task SoftDeleteLessonAsync(Guid courseId, Guid lessonId);
}
