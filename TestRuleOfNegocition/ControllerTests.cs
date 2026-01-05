using Application.DTOs;
using Application.DTOs.Auth;
using Application.DTOs.Courses;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presentation.Controllers;

namespace TestRuleOfNegocition;

/// <summary>
/// Unit tests for API Controllers.
/// These tests validate controller behavior and HTTP response codes without testing database or external services.
/// </summary>
public class ControllerTests
{
    // ==================== AUTH CONTROLLER TESTS ====================

    /// <summary>
    /// Test: Successful user registration should return 200 OK with token
    /// </summary>
    [Fact]
    public async Task Register_WithValidData_ShouldReturnOkWithToken()
    {
        // Arrange
        var mockAuthService = new Mock<IAuthService>();
        var registerDto = new RegisterDto
        {
            Email = "newuser@example.com",
            Password = "SecurePassword123",
            ConfirmPassword = "SecurePassword123",
            FullName = "Test User"
        };

        var expectedResponse = new AuthResponseDto
        {
            Token = "jwt-token-here",
            Email = "newuser@example.com",
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        };

        mockAuthService
            .Setup(x => x.RegisterAsync(registerDto))
            .ReturnsAsync(expectedResponse);

        var controller = new AuthController(mockAuthService.Object);

        // Act
        var result = await controller.Register(registerDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(200, okResult.StatusCode);
        Assert.NotNull(okResult.Value);
        
        var returnedResponse = Assert.IsType<AuthResponseDto>(okResult.Value);
        Assert.Equal("newuser@example.com", returnedResponse.Email);
        Assert.Equal("jwt-token-here", returnedResponse.Token);

        mockAuthService.Verify(x => x.RegisterAsync(registerDto), Times.Once);
    }

    /// <summary>
    /// Test: Login with valid credentials should return 200 OK with token
    /// </summary>
    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnOkWithToken()
    {
        // Arrange
        var mockAuthService = new Mock<IAuthService>();
        var loginDto = new LoginDto
        {
            Email = "user@example.com",
            Password = "Password123"
        };

        var expectedResponse = new AuthResponseDto
        {
            Token = "jwt-token-here",
            Email = "user@example.com",
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        };

        mockAuthService
            .Setup(x => x.LoginAsync(loginDto))
            .ReturnsAsync(expectedResponse);

        var controller = new AuthController(mockAuthService.Object);

        // Act
        var result = await controller.Login(loginDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(200, okResult.StatusCode);
        
        var returnedResponse = Assert.IsType<AuthResponseDto>(okResult.Value);
        Assert.Equal("user@example.com", returnedResponse.Email);
        Assert.NotEmpty(returnedResponse.Token);

        mockAuthService.Verify(x => x.LoginAsync(loginDto), Times.Once);
    }

    // ==================== COURSE CONTROLLER TESTS ====================

    /// <summary>
    /// Test: Creating a new course should return 201 Created with location header
    /// </summary>
    [Fact]
    public async Task CreateCourse_WithValidTitle_ShouldReturn201Created()
    {
        // Arrange
        var mockCourseService = new Mock<ICourseService>();
        var courseTitle = "Advanced C# Course";
        var courseId = Guid.NewGuid();

        mockCourseService
            .Setup(x => x.CreateCourseAsync(courseTitle))
            .ReturnsAsync(courseId);

        var controller = new CourseController(mockCourseService.Object);

        // Act
        var result = await controller.CreateCourse(courseTitle);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(201, createdResult.StatusCode);
        Assert.Equal(nameof(CourseController.GetSummary), createdResult.ActionName);
        
        mockCourseService.Verify(x => x.CreateCourseAsync(courseTitle), Times.Once);
    }

    /// <summary>
    /// Test: Publishing a course without lessons should return 400 Bad Request
    /// </summary>
    [Fact]
    public async Task PublishCourse_WithoutLessons_ShouldReturn400BadRequest()
    {
        // Arrange
        var mockCourseService = new Mock<ICourseService>();
        var courseId = Guid.NewGuid();

        mockCourseService
            .Setup(x => x.PublishCourseAsync(courseId))
            .ThrowsAsync(new InvalidOperationException("A course must have at least one active lesson to be published."));

        var controller = new CourseController(mockCourseService.Object);

        // Act
        var result = await controller.PublishCourse(courseId);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
        
        mockCourseService.Verify(x => x.PublishCourseAsync(courseId), Times.Once);
    }

    /// <summary>
    /// Test: Deleting a course should return 204 No Content
    /// </summary>
    [Fact]
    public async Task DeleteCourse_WithValidId_ShouldReturn204NoContent()
    {
        // Arrange
        var mockCourseService = new Mock<ICourseService>();
        var courseId = Guid.NewGuid();

        mockCourseService
            .Setup(x => x.SoftDeleteCourseAsync(courseId))
            .Returns(Task.CompletedTask);

        var controller = new CourseController(mockCourseService.Object);

        // Act
        var result = await controller.DeleteCourse(courseId);

        // Assert
        var noContentResult = Assert.IsType<NoContentResult>(result);
        Assert.Equal(204, noContentResult.StatusCode);
        
        mockCourseService.Verify(x => x.SoftDeleteCourseAsync(courseId), Times.Once);
    }

    // ==================== LESSON CONTROLLER TESTS ====================

    /// <summary>
    /// Test: Adding a lesson with valid data should return 201 Created
    /// </summary>
    [Fact]
    public async Task AddLesson_WithValidData_ShouldReturn201Created()
    {
        // Arrange
        var mockLessonService = new Mock<ILessonService>();
        var courseId = Guid.NewGuid();
        var lessonId = Guid.NewGuid();
        
        var lessonInput = new Presentation.Controllers.LessonInputDto("Introduction to C#", 1);

        mockLessonService
            .Setup(x => x.AddLessonAsync(courseId, lessonInput.Title, lessonInput.Order))
            .ReturnsAsync(lessonId);

        var controller = new LessonController(mockLessonService.Object);

        // Act
        var result = await controller.AddLesson(courseId, lessonInput);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(201, createdResult.StatusCode);
        Assert.Equal(nameof(LessonController.GetLesson), createdResult.ActionName);
        
        mockLessonService.Verify(
            x => x.AddLessonAsync(courseId, lessonInput.Title, lessonInput.Order), 
            Times.Once);
    }

    /// <summary>
    /// Test: Adding a lesson with empty title should return 400 Bad Request
    /// </summary>
    [Fact]
    public async Task AddLesson_WithEmptyTitle_ShouldReturn400BadRequest()
    {
        // Arrange
        var mockLessonService = new Mock<ILessonService>();
        var courseId = Guid.NewGuid();
        
        var lessonInput = new Presentation.Controllers.LessonInputDto("", 1);

        var controller = new LessonController(mockLessonService.Object);

        // Act
        var result = await controller.AddLesson(courseId, lessonInput);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
        
        mockLessonService.Verify(
            x => x.AddLessonAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<int>()), 
            Times.Never);
    }
}

