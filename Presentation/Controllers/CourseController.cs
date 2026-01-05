using Application.Interfaces;
using Application.DTOs;
using Application.DTOs.Courses;
using Microsoft.AspNetCore.Authorization;
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

        /// <summary>
        /// Obtiene una lista paginada de todos los cursos.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<CourseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<CourseDto>>> GetAllCourses([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var courses = await _courseService.GetAllCoursesAsync(page, pageSize);
            return Ok(courses);
        }

        /// <summary>
        /// Busca cursos con filtros y paginación.
        /// </summary>
        [Authorize]
        [HttpGet("search")]
        [ProducesResponseType(typeof(PagedResult<CourseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<CourseDto>>> SearchCourses(
            [FromQuery] string? q, 
            [FromQuery] string? status, 
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10)
        {
            var results = await _courseService.SearchCoursesAsync(q ?? "", status ?? "", page, pageSize);
            return Ok(results);
        }

        /// <summary>
        /// Obtiene un curso por su ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CourseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CourseDto>> GetCourse(Guid id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null)
                return NotFound();

            return Ok(course);
        }

        /// <summary>
        /// Crea un nuevo curso en estado borrador.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateCourse([FromBody] string title)
        {
            var id = await _courseService.CreateCourseAsync(title);
            return CreatedAtAction(nameof(GetSummary), new { id }, new { id });
        }

        /// <summary>
        /// Publica un curso (requiere al menos una lección activa).
        /// </summary>
        [Authorize]
        [HttpPost("{id}/publish")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Revierte un curso a estado borrador.
        /// </summary>
        [Authorize]
        [HttpPost("{id}/unpublish")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UnpublishCourse(Guid id)
        {
            try
            {
                await _courseService.UnpublishCourseAsync(id);
                return Ok(new { message = "Course unpublished successfully" });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Obtiene un resumen del curso con estadísticas.
        /// </summary>
        [Authorize]
        [HttpGet("{id}/summary")]
        [ProducesResponseType(typeof(CourseSummaryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Elimina lógicamente un curso.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
