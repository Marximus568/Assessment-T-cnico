namespace Application.UseCases.Lessons.CreateLesson;

public class CreateLessonCommand
{
    public Guid CourseId { get; set; }
    public string Title { get; set; }
    public int Order { get; set; }
}