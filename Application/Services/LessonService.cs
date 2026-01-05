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
        // Validation
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Lesson title cannot be empty", nameof(title));

        if (order < 1)
            throw new ArgumentException("Lesson order must be greater than 0", nameof(order));

        // Get course to verify it exists
        var course = await _courseRepository.GetByIdAsync(courseId);
        if (course == null)
            throw new KeyNotFoundException($"Course with ID {courseId} not found");

        // Create lesson using factory method
        var lesson = Domain.Entities.Lesson.Create(courseId, title, order);
        
        // Use the repository method to add lesson directly (avoids concurrency issues)
        await _courseRepository.AddLessonAsync(courseId, lesson);

        return lesson.Id;
    }

    public async Task<LessonDto?> GetLessonAsync(Guid courseId, Guid lessonId)
    {
        var course = await _courseRepository.GetByIdAsync(courseId);
        if (course == null)
            throw new KeyNotFoundException($"Course with ID {courseId} not found");

        var lesson = course.Lessons.FirstOrDefault(l => l.Id == lessonId);
        return lesson == null ? null : _mapper.Map<LessonDto>(lesson);
    }

    public async Task<PagedResult<LessonDto>> GetLessonsByCourseIdAsync(Guid courseId, int page, int pageSize)
    {
        if (page < 1)
            throw new ArgumentException("Page must be greater than 0", nameof(page));

        if (pageSize < 1)
            throw new ArgumentException("PageSize must be greater than 0", nameof(pageSize));

        var course = await _courseRepository.GetByIdAsync(courseId);
        if (course == null)
            throw new KeyNotFoundException($"Course with ID {courseId} not found");

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
        if (newOrder < 1)
            throw new ArgumentException("New order must be greater than 0", nameof(newOrder));

        var course = await _courseRepository.GetByIdAsync(courseId);
        if (course == null)
            throw new KeyNotFoundException($"Course with ID {courseId} not found");

        try
        {
            course.ReorderLesson(lessonId, newOrder);
            await _courseRepository.UpdateAsync(course);
        }
        catch (KeyNotFoundException ex)
        {
            throw new KeyNotFoundException($"Lesson with ID {lessonId} not found in course", ex);
        }
        catch (InvalidOperationException ex)
        {
            throw new InvalidOperationException($"Cannot reorder lesson: {ex.Message}", ex);
        }
    }

    public async Task SoftDeleteLessonAsync(Guid courseId, Guid lessonId)
    {
        var course = await _courseRepository.GetByIdAsync(courseId);
        if (course == null)
            throw new KeyNotFoundException($"Course with ID {courseId} not found");

        var lesson = course.Lessons.FirstOrDefault(l => l.Id == lessonId);
        if (lesson == null)
            throw new KeyNotFoundException($"Lesson with ID {lessonId} not found in course");

        lesson.SoftDelete();
        await _courseRepository.UpdateAsync(course);
    }
}
