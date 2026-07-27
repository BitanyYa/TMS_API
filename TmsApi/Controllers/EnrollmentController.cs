using Microsoft.AspNetCore.Mvc;

namespace TmsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnrollmentController : ControllerBase
{
    private readonly IEnrollmentService _service;

    public EnrollmentController(IEnrollmentService service)
    {
        _service = service;
    }
}

[HttpPost]
public async Task<IActionResult> Enroll(string studentId, string courseCode)
{
    var result = await _service.EnrollAsync(studentId, courseCode);
    return Ok(result);
}