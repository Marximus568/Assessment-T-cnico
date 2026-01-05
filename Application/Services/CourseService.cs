using Application.DTOs.Courses;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;

    public CourseService(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<Guid> CreateCourseAsync(string title)
    {
        var course = new Course(title);
        await _courseRepository.AddAsync(course);
        return course.Id;
    }

    public async Task PublishCourseAsync(Guid courseId)
    {
        var course = await _courseRepository.GetByIdAsync(courseId);
        if (course == null) throw new KeyNotFoundException("Course not found");

        course.Publish();
        await _courseRepository.UpdateAsync(course);
    }

    public async Task<CourseSummaryDto> GetCourseSummaryAsync(Guid courseId)
    {
        var course = await _courseRepository.GetByIdAsync(courseId);
        if (course == null) throw new KeyNotFoundException("Course not found");

        return new CourseSummaryDto
        {
            Id = course.Id,
            Title = course.Title,
            Status = course.Status.ToString(),
            TotalLessons = course.Lessons.Count,
            LastModified = course.UpdatedAt
        };
    }

    public async Task SoftDeleteCourseAsync(Guid courseId)
    {
        var course = await _courseRepository.GetByIdAsync(courseId);
        if (course == null) throw new KeyNotFoundException("Course not found");

        course.SoftDelete();
        await _courseRepository.UpdateAsync(course);
    }
}
