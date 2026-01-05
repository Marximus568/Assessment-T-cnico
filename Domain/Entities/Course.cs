using Domain.Enums;

namespace Domain.Entities
{
    public class Course
    {
        // Identity
        public Guid Id { get; private set; }

        // Properties
        public string Title { get; private set; }
        public CourseStatus Status { get; private set; }
        public bool IsDeleted { get; private set; }

        // Audit fields
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        // Navigation (Domain-level relationship)
        private readonly List<Lesson> _lessons = new();
        public IReadOnlyCollection<Lesson> Lessons => _lessons.Where(l => !l.IsDeleted).ToList().AsReadOnly();

        // Constructor for domain creation
        public Course(string title)
        {
            Id = Guid.NewGuid();
            Title = title;
            Status = CourseStatus.Draft;
            IsDeleted = false;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        // Behavior (business rules)
        public void Publish()
        {
            if (Status == CourseStatus.Published)
                return;

            if (!_lessons.Any(l => !l.IsDeleted))
                throw new InvalidOperationException("A course must have at least one active lesson to be published.");

            Status = CourseStatus.Published;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Unpublish()
        {
            if (Status == CourseStatus.Draft)
                return;

            Status = CourseStatus.Draft;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddLesson(string title, int order)
        {
            // Intelligent Reordering Logic: Shift subsequent lessons
            var subsequentLessons = _lessons
                .Where(l => !l.IsDeleted && l.Order >= order)
                .OrderBy(l => l.Order)
                .ToList();

            foreach (var subsequentLesson in subsequentLessons)
            {
                subsequentLesson.UpdateOrder(subsequentLesson.Order + 1);
            }

            var lesson = new Lesson(Id, title, order);
            _lessons.Add(lesson);
            UpdatedAt = DateTime.UtcNow;
        }

        public void ReorderLesson(Guid lessonId, int newOrder)
        {
            var lesson = _lessons.FirstOrDefault(l => l.Id == lessonId && !l.IsDeleted);
            if (lesson == null)
                throw new KeyNotFoundException("Lesson not found or deleted.");

            if (lesson.Order == newOrder)
                return;

            // Intelligent Reordering Logic: Shift other lessons
            if (newOrder < lesson.Order)
            {
                // Moving up: Increment lessons between newOrder and current lesson.Order-1
                var affected = _lessons
                    .Where(l => !l.IsDeleted && l.Id != lessonId && l.Order >= newOrder && l.Order < lesson.Order)
                    .ToList();
                foreach (var l in affected) l.UpdateOrder(l.Order + 1);
            }
            else
            {
                // Moving down: Decrement lessons between current lesson.Order+1 and newOrder
                var affected = _lessons
                    .Where(l => !l.IsDeleted && l.Id != lessonId && l.Order > lesson.Order && l.Order <= newOrder)
                    .ToList();
                foreach (var l in affected) l.UpdateOrder(l.Order - 1);
            }

            lesson.UpdateOrder(newOrder);
            UpdatedAt = DateTime.UtcNow;
        }

        public void SoftDelete()
        {
            IsDeleted = true;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}