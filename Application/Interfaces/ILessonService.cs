namespace Application.Interfaces;

public interface ILessonService
{
    Task<Guid> AddLessonAsync(Guid courseId, string title, int order);
    Task ReorderLessonAsync(Guid courseId, Guid lessonId, int newOrder);
    Task SoftDeleteLessonAsync(Guid courseId, Guid lessonId);
}
