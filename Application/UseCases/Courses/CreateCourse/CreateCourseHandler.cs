using Application.Interfaces;
using Domain.Entities;

namespace Application.UseCases.Courses.CreateCourse;

public class CreateCourseHandler
{
    private readonly ICourseRepository _courseRepository;

    public CreateCourseHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<Guid> HandleAsync(CreateCourseCommand command)
    {
        var course = new Course(command.Title);

        await _courseRepository.AddAsync(course);

        return course.Id;
    }
}