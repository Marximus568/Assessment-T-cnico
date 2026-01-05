using Application.DTOs;
using Application.Interfaces;
using AutoMapper;

namespace Application.Services;

public class LessonService : ILessonService
{
    private readonly ICourseRepository _courseRepository;
    private readonly IMapper _mapper;

    public LessonService(ICourseRepository courseRepository, IMapper mapper)
    {
        _courseRepository = courseRepository;
        _mapper = mapper;
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

    public async Task<LessonDto?> GetLessonAsync(Guid courseId, Guid lessonId)
    {
        var course = await _courseRepository.GetByIdAsync(courseId);
        if (course == null) throw new KeyNotFoundException("Course not found");

        var lesson = course.Lessons.FirstOrDefault(l => l.Id == lessonId);
        if (lesson == null) return null;

        return _mapper.Map<LessonDto>(lesson);
    }

    public async Task<PagedResult<LessonDto>> GetLessonsByCourseIdAsync(Guid courseId, int page, int pageSize)
    {
        var course = await _courseRepository.GetByIdAsync(courseId);
        if (course == null) throw new KeyNotFoundException("Course not found");

        var lessons = course.Lessons
            .OrderBy(l => l.Order)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var dtos = _mapper.Map<IEnumerable<LessonDto>>(lessons);
        return new PagedResult<LessonDto>(dtos, course.Lessons.Count, page, pageSize);
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
