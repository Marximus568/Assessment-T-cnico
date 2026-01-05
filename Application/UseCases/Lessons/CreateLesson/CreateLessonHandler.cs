using Application.Interfaces;
using Domain.Entities;

namespace Application.UseCases.Lessons.CreateLesson;

public class CreateLessonHandler
{
    private readonly ILessonRepository _lessonRepository;
    private readonly ICourseRepository _courseRepository;

    public CreateLessonHandler(
        ILessonRepository lessonRepository,
        ICourseRepository courseRepository)
    {
        _lessonRepository = lessonRepository;
        _courseRepository = courseRepository;
    }

    public async Task<Guid> HandleAsync(CreateLessonCommand command)
    {
        var course = await _courseRepository.GetByIdAsync(command.CourseId);

        if (course == null)
            throw new KeyNotFoundException("Course not found");

        course.AddLesson(command.Title, command.Order);

        await _courseRepository.UpdateAsync(course);

        // Since Lesson is internal and added via Course, we need to find it back or return it from AddLesson.
        // For simplicity in this assessment, I'll return the ID of the lesson just added.
        var lesson = course.Lessons.First(l => l.Order == command.Order && l.Title == command.Title);
        return lesson.Id;
    }
}