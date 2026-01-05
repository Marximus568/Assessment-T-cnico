using Application.Interfaces;
using Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/v1/Course/{courseId}/[controller]")]
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
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PagedResult<LessonDto>>> GetLessons(Guid courseId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var lessons = await _lessonService.GetLessonsByCourseIdAsync(courseId, page, pageSize);
                return Ok(lessons);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Adds a new lesson to a course.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddLesson(Guid courseId, [FromBody] LessonInputDto input)
        {
            try
            {
                var lessonId = await _lessonService.AddLessonAsync(courseId, input.Title, input.Order);
                return CreatedAtAction(nameof(AddLesson), new { courseId, id = lessonId }, new { id = lessonId });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Changes the order of a lesson within the course.
        /// </summary>
        [HttpPut("{lessonId}/reorder")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ReorderLesson(Guid courseId, Guid lessonId, [FromBody] int newOrder)
        {
            try
            {
                await _lessonService.ReorderLessonAsync(courseId, lessonId, newOrder);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Soft deletes a lesson.
        /// </summary>
        [HttpDelete("{lessonId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteLesson(Guid courseId, Guid lessonId)
        {
            try
            {
                await _lessonService.SoftDeleteLessonAsync(courseId, lessonId);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }

    public record LessonInputDto(string Title, int Order);
}
