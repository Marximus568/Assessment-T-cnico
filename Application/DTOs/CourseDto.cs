using Domain.Enums;

namespace Application.DTOs;
public class CourseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public CourseStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}