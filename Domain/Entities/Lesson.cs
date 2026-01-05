namespace Domain.Entities
{
    public class Lesson
    {
        // Identity
        public Guid Id { get; private set; }

        // Foreign Key (Domain-level reference)
        public Guid CourseId { get; private set; }

        // Properties
        public string Title { get; private set; }
        public int Order { get; private set; }
        public bool IsDeleted { get; private set; }

        // Audit fields
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        // Constructor
        internal Lesson(Guid courseId, string title, int order)
        {
            Id = Guid.NewGuid();
            CourseId = courseId;
            Title = title;
            Order = order;
            IsDeleted = false;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        // Factory method for creating lessons
        public static Lesson Create(Guid courseId, string title, int order)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Lesson title cannot be empty", nameof(title));

            if (order < 1)
                throw new ArgumentException("Lesson order must be greater than 0", nameof(order));

            return new Lesson(courseId, title, order);
        }

        // Behavior
        public void UpdateOrder(int newOrder)
        {
            Order = newOrder;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SoftDelete()
        {
            IsDeleted = true;
            UpdatedAt = DateTime.UtcNow;
        }
        
    }
}