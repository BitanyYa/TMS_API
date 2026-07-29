using System.ComponentModel.DataAnnotations;

namespace TMS_API.Models;

public record CreateEnrollmentRequest
{
    [Required]
    public string StudentId { get; init; } = string.Empty;

    [Required]
    public string CourseCode { get; init; } = string.Empty;
}