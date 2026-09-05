using Microsoft.AspNetCore.Mvc;

namespace TmsApi.Api.Controllers.V2;

public record GradeSubmissionDto(int StudentId, int CourseId, decimal Score);

[ApiController]
[Route("api/grades")]
public class GradesController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> SubmitGrade([FromBody] GradeSubmissionDto dto, CancellationToken ct)
    {
        // Simulate database commit / validation work (1.5s delay to make exhaustMap testable)
        await Task.Delay(TimeSpan.FromSeconds(1.5), ct);

        var recordId = $"rec-{Random.Shared.Next(10000, 99999)}";

        return Ok(new
        {
            id = recordId,
            success = true
        });
    }
}