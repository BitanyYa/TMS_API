using System.ComponentModel.DataAnnotations;

namespace TmsApi.Configuration;

public class EnrollmentOptions
{
    [Required]
    public string Department { get; set; } = string.Empty;

    [Range(1, 100)]
    public int MaxStudentsPerCourse { get; set; }
}