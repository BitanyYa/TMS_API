using Microsoft.AspNetCore.Mvc;
using TmsApi.Dtos;
using TmsApi.Services;

namespace TmsApi.Controllers;

[ApiController]
[Route("api/courses")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CoursesController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    // GET /api/courses
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var courses = await _courseService.GetAllAsync(ct);
        return Ok(courses);
    }

    // GET /api/courses/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var course = await _courseService.GetByIdAsync(id, ct);

        return course is not null
            ? Ok(course)
            : NotFound();
    }

    // POST /api/courses
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCourseRequest request, CancellationToken ct)
    {
        var course = await _courseService.CreateAsync(request, ct);

        return CreatedAtAction(
            nameof(GetById),
            new { id = course.Id },
            course);
    }

    // GET /api/courses/code-exists?code=CS-101
    [HttpGet("code-exists")]
    public async Task<IActionResult> CodeExists(string code, CancellationToken ct)
    {
        var exists = await _courseService.CodeExistsAsync(code, ct);
        return Ok(new { exists });
    }
}
