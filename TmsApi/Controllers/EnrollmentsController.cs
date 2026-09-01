using Microsoft.AspNetCore.Mvc;
using TmsApi.Entities;
using TmsApi.Services;

namespace TmsApi.Controllers;

[ApiController]
[Route("api/enrollments")]
public class EnrollmentsController : ControllerBase
{
    private readonly IEnrollmentService _enrollmentService;

    public EnrollmentsController(IEnrollmentService enrollmentService)
    {
        _enrollmentService = enrollmentService;
    }

    // GET /api/enrollments
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var enrollments = await _enrollmentService.GetAllAsync();
        return Ok(enrollments);
    }

    // GET /api/enrollments/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var record = await _enrollmentService.GetByIdAsync(id);

        return record is not null
            ? Ok(record)
            : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEnrollmentRequest request)
    {
        var record = await _enrollmentService.EnrollAsync(
            request.StudentId,
            request.CourseCode);

        return CreatedAtAction(
            nameof(GetById),
            new { id = record.Id },
            record);
    }

    [HttpGet("crash")]
    public IActionResult Crash()
    {
        throw new InvalidOperationException("This is a test exception.");
    }
}