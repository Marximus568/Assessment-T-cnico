using Application.Interfaces;
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

        [HttpPost]
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

        [HttpPut("{lessonId}/reorder")]
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

        [HttpDelete("{lessonId}")]
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
