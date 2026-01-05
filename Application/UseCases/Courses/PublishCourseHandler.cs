using Application.Interfaces;

namespace Application.UseCases.Courses;

public class PublishCourseHandler
{
    private readonly ICourseRepository _courseRepository;

    public PublishCourseHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task HandleAsync(Guid courseId)
    {
        var course = await _courseRepository.GetByIdAsync(courseId);

        if (course == null)
            throw new KeyNotFoundException("Course not found");

        course.Publish();

        await _courseRepository.UpdateAsync(course);
    }
}