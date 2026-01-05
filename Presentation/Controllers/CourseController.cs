using Application.Interfaces;
using Application.DTOs.Courses;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCourse([FromBody] string title)
        {
            var id = await _courseService.CreateCourseAsync(title);
            return CreatedAtAction(nameof(GetSummary), new { id }, new { id });
        }

        [HttpPost("{id}/publish")]
        public async Task<IActionResult> PublishCourse(Guid id)
        {
            try
            {
                await _courseService.PublishCourseAsync(id);
                return Ok();
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

        [HttpGet("{id}/summary")]
        public async Task<ActionResult<CourseSummaryDto>> GetSummary(Guid id)
        {
            try
            {
                var summary = await _courseService.GetCourseSummaryAsync(id);
                return Ok(summary);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(Guid id)
        {
            try
            {
                await _courseService.SoftDeleteCourseAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
