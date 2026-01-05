using AutoMapper;
using Application.DTOs;
using Application.DTOs.Courses;
using Domain.Entities;

namespace Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Course, CourseDto>();
        CreateMap<Course, CourseSummaryDto>()
            .ForMember(dest => dest.TotalLessons, opt => opt.MapFrom(src => src.Lessons.Count))
            .ForMember(dest => dest.LastModified, opt => opt.MapFrom(src => src.UpdatedAt));
        
        CreateMap<Lesson, LessonDto>();
    }
}
