using Application.DTOs.Courses;
using Application.Interfaces;

namespace Application.UseCases.Courses;

public class GetCourseSummaryHandler
{
    private readonly ICourseRepository _courseRepository;

    public GetCourseSummaryHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<CourseSummaryDto> HandleAsync(Guid courseId)
    {
        var course = await _courseRepository.GetByIdAsync(courseId);

        if (course == null)
            throw new KeyNotFoundException("Course not found");

        return new CourseSummaryDto
        {
            Id = course.Id,
            Title = course.Title,
            Status = course.Status.ToString(),
            TotalLessons = course.Lessons.Count,
            LastModified = course.UpdatedAt
        };
    }
}
