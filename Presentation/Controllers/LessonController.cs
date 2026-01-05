using Application.Interfaces;
using Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/v1/courses/{courseId}/[controller]s")]
    [ApiController]
    public class LessonController : ControllerBase
    {
        private readonly ILessonService _lessonService;

        public LessonController(ILessonService lessonService)
        {
            _lessonService = lessonService;
        }

        /// <summary>
        /// Lists the lessons of a course with pagination.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<LessonDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PagedResult<LessonDto>>> GetLessons(Guid courseId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var lessons = await _lessonService.GetLessonsByCourseIdAsync(courseId, page, pageSize);
                return Ok(lessons);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Gets a specific lesson.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(LessonDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LessonDto>> GetLesson(Guid courseId, Guid id)
        {
            try
            {
                var lesson = await _lessonService.GetLessonAsync(courseId, id);
                if (lesson == null) return NotFound(new { error = "Lesson not found" });
                return Ok(lesson);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Adds a new lesson to a course.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddLesson(Guid courseId, [FromBody] LessonInputDto input)
        {
            try
            {
                // Validate input
                if (input == null)
                    return BadRequest(new { error = "Request body is required" });

                if (string.IsNullOrWhiteSpace(input.Title))
                    return BadRequest(new { error = "Lesson title cannot be empty" });

                if (input.Order < 1)
                    return BadRequest(new { error = "Lesson order must be greater than 0" });

                var lessonId = await _lessonService.AddLessonAsync(courseId, input.Title, input.Order);
                return CreatedAtAction(nameof(GetLesson), new { courseId, id = lessonId }, new { id = lessonId });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                // In production, log the exception
                return StatusCode(500, new { error = "Internal server error while creating lesson", details = ex.Message });
            }
        }

        /// <summary>
        /// Changes the order of a lesson within the course.
        /// </summary>
        [HttpPut("{lessonId}/reorder")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ReorderLesson(Guid courseId, Guid lessonId, [FromBody] int newOrder)
        {
            try
            {
                if (newOrder < 1)
                    return BadRequest(new { error = "New order must be greater than 0" });

                await _lessonService.ReorderLessonAsync(courseId, lessonId, newOrder);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                // In production, log the exception
                return StatusCode(500, new { error = "Internal server error while reordering lesson", details = ex.Message });
            }
        }

        /// <summary>
        /// Soft deletes a lesson.
        /// </summary>
        [HttpDelete("{lessonId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteLesson(Guid courseId, Guid lessonId)
        {
            try
            {
                await _lessonService.SoftDeleteLessonAsync(courseId, lessonId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                // In production, log the exception
                return StatusCode(500, new { error = "Internal server error while deleting lesson", details = ex.Message });
            }
        }
    }

    public record LessonInputDto(string Title, int Order);
}
