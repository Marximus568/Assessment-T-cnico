using Domain.Entities;
using Domain.Enums;

namespace TestRuleOfNegocition;

/// <summary>
/// Unit tests for business rules related to Course and Lesson entities.
/// These tests validate critical domain logic without dependencies on external services.
/// </summary>
public class CourseAndLessonBusinessRulesTests
{
    // ==================== COURSE PUBLICATION TESTS ====================
    
    /// <summary>
    /// Business Rule: A course with at least one lesson can be published
    /// </summary>
    [Fact]
    public void PublishCourse_WithLessons_ShouldSucceed()
    {
        // Arrange
        var course = new Course("Advanced C# Course");
        course.AddLesson("Introduction to Async/Await", 1);
        
        // Act & Assert
        var exception = Record.Exception(() => course.Publish());
        
        Assert.Null(exception);
        Assert.Equal(CourseStatus.Published, course.Status);
    }

    /// <summary>
    /// Business Rule: A course without lessons cannot be published
    /// Expected behavior: Should throw InvalidOperationException
    /// </summary>
    [Fact]
    public void PublishCourse_WithoutLessons_ShouldFail()
    {
        // Arrange
        var course = new Course("Empty Course");
        
        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => course.Publish());
        
        Assert.NotNull(exception);
        Assert.Equal("A course must have at least one active lesson to be published.", exception.Message);
        Assert.Equal(CourseStatus.Draft, course.Status);
    }

    // ==================== LESSON CREATION TESTS ====================
    
    /// <summary>
    /// Business Rule: A lesson can be created with a unique order within a course
    /// </summary>
    [Fact]
    public void CreateLesson_WithUniqueOrder_ShouldSucceed()
    {
        // Arrange
        var course = new Course("Python Fundamentals");
        var lessonTitle = "Variables and Data Types";
        var lessonOrder = 1;
        
        // Act & Assert
        var exception = Record.Exception(() => course.AddLesson(lessonTitle, lessonOrder));
        
        Assert.Null(exception);
        Assert.Single(course.Lessons);
        var addedLesson = course.Lessons.First();
        Assert.Equal(lessonTitle, addedLesson.Title);
        Assert.Equal(lessonOrder, addedLesson.Order);
    }

    /// <summary>
    /// Business Rule: Adding a lesson with an existing order should shift subsequent lessons.
    /// </summary>
    [Fact]
    public void CreateLesson_WithDuplicateOrder_ShouldReorder()
    {
        // Arrange
        var course = new Course("JavaScript Essentials");
        course.AddLesson("Getting Started", 1);
        
        // Act
        course.AddLesson("Prerequisites", 1);
        
        // Assert
        Assert.Equal(2, course.Lessons.Count);
        var lessons = course.Lessons.OrderBy(l => l.Order).ToList();
        Assert.Equal("Prerequisites", lessons[0].Title);
        Assert.Equal(1, lessons[0].Order);
        Assert.Equal("Getting Started", lessons[1].Title);
        Assert.Equal(2, lessons[1].Order);
    }

    // ==================== SOFT DELETE TESTS ====================
    
    /// <summary>
    /// Business Rule: Deleting a course should be a soft delete (IsDeleted = true)
    /// The course record should still exist but marked as deleted
    /// </summary>
    [Fact]
    public void DeleteCourse_ShouldBeSoftDelete()
    {
        // Arrange
        var course = new Course("Web Development Masterclass");
        var courseId = course.Id;
        Assert.False(course.IsDeleted);
        
        // Act
        course.SoftDelete();
        
        // Assert
        Assert.True(course.IsDeleted);
        Assert.Equal(courseId, course.Id); // ID should remain the same
        Assert.Equal("Web Development Masterclass", course.Title); // Data should remain intact
    }

    // ==================== ADDITIONAL BUSINESS RULE TESTS ====================
    
    /// <summary>
    /// Business Rule: Soft-deleted lessons should not be counted when publishing a course
    /// A course cannot be published if all its lessons are deleted
    /// </summary>
    [Fact]
    public void PublishCourse_WithOnlyDeletedLessons_ShouldFail()
    {
        // Arrange
        var course = new Course("Data Science Course");
        course.AddLesson("NumPy Basics", 1);
        
        // Get the lesson and soft delete it
        var lesson = course.Lessons.First();
        lesson.SoftDelete();
        
        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => course.Publish());
        
        Assert.NotNull(exception);
        Assert.Equal("A course must have at least one active lesson to be published.", exception.Message);
    }

    /// <summary>
    /// Business Rule: Multiple lessons can be added with sequential, non-conflicting orders
    /// </summary>
    [Fact]
    public void CreateMultipleLessons_WithSequentialOrders_ShouldSucceed()
    {
        // Arrange
        var course = new Course("Machine Learning Fundamentals");
        
        // Act
        course.AddLesson("Linear Regression", 1);
        course.AddLesson("Logistic Regression", 2);
        course.AddLesson("Decision Trees", 3);
        
        // Assert
        Assert.Equal(3, course.Lessons.Count);
        Assert.True(course.Lessons.All(l => !l.IsDeleted));
        
        var orderedLessons = course.Lessons.OrderBy(l => l.Order).ToList();
        Assert.Equal(1, orderedLessons[0].Order);
        Assert.Equal(2, orderedLessons[1].Order);
        Assert.Equal(3, orderedLessons[2].Order);
    }

    /// <summary>
    /// Business Rule: A course status should be Draft when initially created
    /// </summary>
    [Fact]
    public void CreateCourse_ShouldHaveDraftStatus()
    {
        // Arrange & Act
        var course = new Course("Cloud Computing Basics");
        
        // Assert
        Assert.Equal(CourseStatus.Draft, course.Status);
        Assert.False(course.IsDeleted);
        Assert.NotEqual(Guid.Empty, course.Id);
    }

    /// <summary>
    /// Business Rule: Unpublishing a published course should revert it to Draft status
    /// </summary>
    [Fact]
    public void UnpublishCourse_FromPublished_ShouldRevertToDraft()
    {
        // Arrange
        var course = new Course("Database Design");
        course.AddLesson("Normalization", 1);
        course.Publish();
        Assert.Equal(CourseStatus.Published, course.Status);
        
        // Act
        course.Unpublish();
        
        // Assert
        Assert.Equal(CourseStatus.Draft, course.Status);
    }

    /// <summary>
    /// Business Rule: Reordering a lesson to an occupied order should shift other lessons.
    /// </summary>
    [Fact]
    public void ReorderLesson_ToDuplicateOrder_ShouldReorder()
    {
        // Arrange
        var course = new Course("Software Architecture");
        course.AddLesson("Design Patterns", 1); // Becomes 2 after SOLID added
        course.AddLesson("SOLID Principles", 2); // Becomes 3 after Clean Code added
        course.AddLesson("Clean Code", 3);
        
        // At this point: DP=1, SOLID=2, CC=3 (Wait, no, AddLesson shifts)
        // Let's re-arrange the Arrange for clarity:
        course = new Course("Software Architecture");
        course.AddLesson("Design Patterns", 1);
        course.AddLesson("SOLID Principles", 2);
        course.AddLesson("Clean Code", 3);
        
        var cleanCodeLine = course.Lessons.First(l => l.Title == "Clean Code");
        
        // Act
        course.ReorderLesson(cleanCodeLine.Id, 1);
        
        // Assert
        var lessons = course.Lessons.OrderBy(l => l.Order).ToList();
        Assert.Equal("Clean Code", lessons[0].Title);
        Assert.Equal(1, lessons[0].Order);
        Assert.Equal("Design Patterns", lessons[1].Title);
        Assert.Equal(2, lessons[1].Order);
        Assert.Equal("SOLID Principles", lessons[2].Title);
        Assert.Equal(3, lessons[2].Order);
    }

    /// <summary>
    /// Business Rule: Successfully reordering a lesson should update its order
    /// </summary>
    [Fact]
    public void ReorderLesson_ToValidOrder_ShouldSucceed()
    {
        // Arrange
        var course = new Course("DevOps Fundamentals");
        course.AddLesson("Docker Basics", 1);
        course.AddLesson("Kubernetes Introduction", 2);
        course.AddLesson("CI/CD Pipelines", 3);
        
        var firstLesson = course.Lessons.First(l => l.Order == 1);
        
        // Act
        course.ReorderLesson(firstLesson.Id, 4);
        
        // Assert
        var reorderedLesson = course.Lessons.First(l => l.Id == firstLesson.Id);
        Assert.Equal(4, reorderedLesson.Order);
    }
}