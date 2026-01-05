namespace Application.DTOs;

public class LessonDto
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string Title { get; set; }
    public int Order { get; set; }
    public DateTime CreatedAt { get; set; }
}