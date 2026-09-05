using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TmsApi.Api.Hubs;
using TmsApi.Application.Hubs;

namespace TmsApi.Api.Controllers;

public record EnrollmentDto(
    string Id,
    int StudentId,
    string StudentName,
    int CourseId,
    string CourseName,
    string Status,
    string EnrolledAt
);

public record UpdateEnrollmentStatusRequest(string Status);

[ApiController]
[Route("api/[controller]")]
public class EnrollmentsController : ControllerBase
{
    private readonly IHubContext<TmsHub, ITmsHubClient> _hubContext;

    private static readonly List<EnrollmentDto> _enrollments = new()
    {
        new("1", 101, "Liya Kebede", 1, "CS-101: Intro to CS", "Pending", DateTime.UtcNow.AddDays(-5).ToString("o")),
        new("2", 102, "Dawit Getachew", 2, "CS-201: Data Structures", "Pending", DateTime.UtcNow.AddDays(-4).ToString("o")),
        new("3", 103, "Abeba Bikila", 1, "CS-101: Intro to CS", "Approved", DateTime.UtcNow.AddDays(-3).ToString("o")),
        new("4", 104, "Yonas Tesfaye", 3, "MAT-101: Calculus I", "Approved", DateTime.UtcNow.AddDays(-2).ToString("o")),
        new("5", 105, "Sara Mengistu", 2, "CS-201: Data Structures", "Rejected", DateTime.UtcNow.AddDays(-1).ToString("o")),
        new("6", 106, "Biruk Hailu", 1, "CS-101: Intro to CS", "Pending", DateTime.UtcNow.ToString("o"))
    };

    public EnrollmentsController(IHubContext<TmsHub, ITmsHubClient> hubContext)
    {
        _hubContext = hubContext;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_enrollments);
    }

    [HttpPost("{id}/approve")]
    public async Task<IActionResult> Approve(string id)
    {
        return await UpdateStatus(id, new UpdateEnrollmentStatusRequest("Approved"));
    }

    [HttpPut("{id}/status")]
    [HttpPost("{id}/status")]
    public async Task<IActionResult> UpdateStatus(string id, [FromBody] UpdateEnrollmentStatusRequest request)
    {
        // Error Simulation for Optimistic Rollback Testing:
        // ID "5" or IDs containing "error" or invalid statuses trigger an RFC 7807 ProblemDetails error
        if (id == "5" || id.Contains("error", StringComparison.OrdinalIgnoreCase))
        {
            return Problem(
                title: "Status Update Conflict",
                detail: $"Enrollment #{id} cannot be modified because it has already been locked by administration.",
                statusCode: StatusCodes.Status409Conflict,
                type: "https://tms.local/errors/status-conflict"
            );
        }

        var index = _enrollments.FindIndex(e => e.Id == id);
        if (index == -1)
        {
            return Problem(
                title: "Enrollment Not Found",
                detail: $"No enrollment record found with ID '{id}'.",
                statusCode: StatusCodes.Status404NotFound,
                type: "https://tms.local/errors/not-found"
            );
        }

        var existing = _enrollments[index];
        var updated = existing with { Status = request.Status };
        _enrollments[index] = updated;

        // Broadcast live SignalR update to connected clients
        await _hubContext.Clients.All.ReceiveEnrollmentStatusUpdated(id, request.Status);

        return Ok(updated);
    }
}
