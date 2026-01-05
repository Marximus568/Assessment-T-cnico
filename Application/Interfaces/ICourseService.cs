using Application.DTOs.Courses;

namespace Application.Interfaces;

public interface ICourseService
{
    Task<Guid> CreateCourseAsync(string title);
    Task PublishCourseAsync(Guid courseId);
    Task<CourseSummaryDto> GetCourseSummaryAsync(Guid courseId);
    Task SoftDeleteCourseAsync(Guid courseId);
}
