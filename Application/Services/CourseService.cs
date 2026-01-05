using Application.DTOs;
using Application.DTOs.Courses;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;

namespace Application.Services;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;
    private readonly IMapper _mapper;

    public CourseService(ICourseRepository courseRepository, IMapper mapper)
    {
        _courseRepository = courseRepository;
        _mapper = mapper;
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

    public async Task UnpublishCourseAsync(Guid courseId)
    {
        var course = await _courseRepository.GetByIdAsync(courseId);
        if (course == null) throw new KeyNotFoundException("Course not found");

        course.Unpublish();
        await _courseRepository.UpdateAsync(course);
    }

    public async Task<CourseSummaryDto> GetCourseSummaryAsync(Guid courseId)
    {
        var course = await _courseRepository.GetByIdAsync(courseId);
        if (course == null) throw new KeyNotFoundException("Course not found");

        return _mapper.Map<CourseSummaryDto>(course);
    }

    public async Task<CourseDto?> GetCourseByIdAsync(Guid courseId)
    {
        var course = await _courseRepository.GetByIdAsync(courseId);
        return course == null ? null : _mapper.Map<CourseDto>(course);
    }

    public async Task<PagedResult<CourseDto>> GetAllCoursesAsync(int page, int pageSize)
    {
        var (items, totalCount) = await _courseRepository.GetAllAsync(page, pageSize);
        var dtos = _mapper.Map<IEnumerable<CourseDto>>(items);
        return new PagedResult<CourseDto>(dtos, totalCount, page, pageSize);
    }

    public async Task<PagedResult<CourseDto>> SearchCoursesAsync(string q, string status, int page, int pageSize)
    {
        var (items, totalCount) = await _courseRepository.SearchAsync(q, status, page, pageSize);
        var dtos = _mapper.Map<IEnumerable<CourseDto>>(items);
        return new PagedResult<CourseDto>(dtos, totalCount, page, pageSize);
    }

    public async Task SoftDeleteCourseAsync(Guid courseId)
    {
        var course = await _courseRepository.GetByIdAsync(courseId);
        if (course == null) throw new KeyNotFoundException("Course not found");

        course.SoftDelete();
        await _courseRepository.UpdateAsync(course);
    }
}
