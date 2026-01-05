using Application.DTOs;
using Application.DTOs.Courses;

namespace Application.Interfaces;

public interface ICourseService
{
    Task<Guid> CreateCourseAsync(string title);
    Task UpdateCourseAsync(Guid courseId, string title);
    Task PublishCourseAsync(Guid courseId);
    Task UnpublishCourseAsync(Guid courseId);
    Task<CourseSummaryDto> GetCourseSummaryAsync(Guid courseId);
    Task<CourseDto?> GetCourseByIdAsync(Guid courseId);
    Task<PagedResult<CourseDto>> GetAllCoursesAsync(int page, int pageSize);
    Task<PagedResult<CourseDto>> SearchCoursesAsync(string q, string status, int page, int pageSize);
    Task SoftDeleteCourseAsync(Guid courseId);
}
