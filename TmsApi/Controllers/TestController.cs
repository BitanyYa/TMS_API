using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TmsApi.Data;

namespace TmsApi.Controllers;

[ApiController]
[Route("api/test")]
public class TestController(TmsDbContext context) : ControllerBase
{
    [HttpGet("deferred")]
    public IActionResult TestDeferred()
    {
        Console.WriteLine("\n>>> STEP 1: Building the query object (no database contact)...");

        var query = context.Students.Where(s => s.GPA >= 3.0m);

        Console.WriteLine(">>> STEP 2: Appending a sorting clause...");

        var orderedQuery = query.OrderBy(s => s.Name);

        Console.WriteLine(">>> STEP 3: Materializing query into a C# List...");

        var results = orderedQuery.ToList();

        Console.WriteLine(">>> STEP 4: Materialization finished. List populated.\n");

        return Ok(results);
    }

    private static bool IsHonorRoll(decimal gpa)
    {
        return gpa >= 3.5m;
    }

    [HttpGet("translation-fail")]
public IActionResult TestTranslationFail()
{
    Console.WriteLine("\n>>> STEP 1: Running non-translatable query.");

    try
    {
        var students = context.Students
            .Where(s => IsHonorRoll(s.GPA))
            .ToList();

        return Ok(students);
    }
    catch (Exception ex)
    {
        Console.WriteLine($">>> EXCEPTION CAUGHT: {ex.Message}\n");

        return BadRequest(new
        {
            Message = ex.Message
        });
    }
}

[HttpGet("active-high-gpa")]
public async Task<IActionResult> GetActiveHighGpa()
{
    var count = await context.Students
        .Where(s => s.IsActive && s.GPA >= 3.0m)
        .CountAsync();

    return Ok(count);
}

[HttpGet("courses-by-enrollment")]
public async Task<IActionResult> GetCoursesByEnrollment()
{
    var list = await context.Courses
        .Select(c => new
        {
            c.Title,
            EnrollmentCount = c.Enrollments.Count
        })
        .OrderByDescending(x => x.EnrollmentCount)
        .ToListAsync();

    return Ok(list);
}

[HttpGet("average-gpa")]
public async Task<IActionResult> GetAverageGpa()
{
    var list = await context.Enrollments
        .GroupBy(e => e.Course.Title)
        .Select(g => new
        {
            Course = g.Key,
            AverageGPA = g.Average(e => e.Student.GPA)
        })
        .ToListAsync();

    return Ok(list);
}

[HttpGet("students-without-enrollments")]
public async Task<IActionResult> GetStudentsWithoutEnrollments()
{
    var list = await context.Students
        .LeftJoin(
            context.Enrollments,
            s => s.Id,
            e => e.StudentId,
            (s, e) => new
            {
                Student = s,
                Enrollment = e
            })
        .Where(x => x.Enrollment == null)
        .Select(x => new
        {
            x.Student.Id,
            x.Student.Name,
            x.Student.RegistrationNumber
        })
        .ToListAsync();

    return Ok(list);
}
}