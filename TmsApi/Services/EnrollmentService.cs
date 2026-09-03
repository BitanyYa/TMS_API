using Microsoft.EntityFrameworkCore;
using TmsApi.Data;
using TmsApi.Dtos;
using TmsApi.Entities;

namespace TmsApi.Services;

public class EnrollmentService : IEnrollmentService
{
    private readonly TmsDbContext _context;

    public EnrollmentService(TmsDbContext context)
    {
        _context = context;
    }

    public async Task<EnrollmentResponseDto?> GetByIdAsync(int courseId, int id, CancellationToken ct)
    {
        var enrollment = await _context.Enrollments
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id && e.CourseId == courseId, ct);

        return enrollment is null
            ? null
            : new EnrollmentResponseDto(enrollment.Id, enrollment.CourseId, enrollment.StudentId, enrollment.EnrolledAt);
    }

    public async Task<EnrollmentResponseDto> CreateAsync(int courseId, EnrollStudentRequest request, CancellationToken ct)
    {
        var enrollment = new Enrollment
        {
            CourseId = courseId,
            StudentId = request.StudentId,
            EnrolledAt = DateTime.UtcNow
        };

        _context.Enrollments.Add(enrollment);
        await _context.SaveChangesAsync(ct);

        return new EnrollmentResponseDto(enrollment.Id, enrollment.CourseId, enrollment.StudentId, enrollment.EnrolledAt);
    }

    public async Task<IReadOnlyList<EnrollmentResponseDto>> GetByCourseAsync(int courseId, CancellationToken ct)
    {
        var enrollments = await _context.Enrollments
            .AsNoTracking()
            .Where(e => e.CourseId == courseId)
            .Select(e => new EnrollmentResponseDto(e.Id, e.CourseId, e.StudentId, e.EnrolledAt))
            .ToListAsync(ct);

        return enrollments;
    }
}



