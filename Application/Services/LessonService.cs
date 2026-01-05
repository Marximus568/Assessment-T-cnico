using Application.Interfaces;

namespace Application.Services;

public class LessonService : ILessonService
{
    private readonly ICourseRepository _courseRepository;

    public LessonService(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<Guid> AddLessonAsync(Guid courseId, string title, int order)
    {
        var course = await _courseRepository.GetByIdAsync(courseId);
        if (course == null) throw new KeyNotFoundException("Course not found");

        course.AddLesson(title, order);
        await _courseRepository.UpdateAsync(course);

        var lesson = course.Lessons.First(l => l.Order == order && l.Title == title);
        return lesson.Id;
    }

    public async Task ReorderLessonAsync(Guid courseId, Guid lessonId, int newOrder)
    {
        var course = await _courseRepository.GetByIdAsync(courseId);
        if (course == null) throw new KeyNotFoundException("Course not found");

        course.ReorderLesson(lessonId, newOrder);
        await _courseRepository.UpdateAsync(course);
    }

    public async Task SoftDeleteLessonAsync(Guid courseId, Guid lessonId)
    {
        var course = await _courseRepository.GetByIdAsync(courseId);
        if (course == null) throw new KeyNotFoundException("Course not found");

        var lesson = course.Lessons.FirstOrDefault(l => l.Id == lessonId);
        if (lesson == null) throw new KeyNotFoundException("Lesson not found");

        lesson.SoftDelete();
        await _courseRepository.UpdateAsync(course);
    }
}
