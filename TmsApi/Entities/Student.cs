namespace TmsApi.Entities;

public class Student
{
    public int Id { get; set; }

    public required string RegistrationNumber { get; set; }

    public required string Name { get; set; }

    public decimal GPA { get; set; }
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;

    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public uint Version { get; set; }
}