using System.ComponentModel.DataAnnotations;

namespace TMS_API.Models;

public record CreateEnrollmentRequest
{
    [Required]
    [StringLength(20)]
    public string StudentId { get; init; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string CourseCode { get; init; } = string.Empty;
}