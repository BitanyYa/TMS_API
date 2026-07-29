using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TmsApi.Services;

namespace TmsApi.Services;


public interface IEnrollmentService
{
    Task<EnrollmentRecord> EnrollAsync(
        string studentId, 
        string courseCode
        );

    Task<EnrollmentRecord?> GetByIdAsync(string id);

    Task<IReadOnlyList<EnrollmentRecord>> GetAllAsync();

    Task<bool> DeleteAsync(string id);
}

public class EnrollmentService : IEnrollmentService
{
    private readonly Dictionary<string, EnrollmentRecord> _store = new();
    private readonly ILogger<EnrollmentService> _logger;
    private readonly IAuditService _auditService;

    public EnrollmentService(ILogger<EnrollmentService> logger, IAuditService auditService)
    {
        _logger = logger;
        _auditService = auditService;
    }

    public Task<EnrollmentRecord> EnrollAsync(string studentId, string courseCode)
    {
    var id = Guid.NewGuid().ToString("N")[..8];

    var record = new EnrollmentRecord(
        id,
        studentId,
        courseCode,
        DateTime.UtcNow);

    _store[id] = record;
    _auditService.Record(
    $"Student {studentId} enrolled in {courseCode}");

    _logger.LogInformation(
    "Enrollment created at {EnrolledAt}",
    record.EnrolledAt);
    
    _logger.LogInformation(
        "Enrolled {StudentId} in {CourseCode} record {EnrollmentId}",
        studentId,
        courseCode,
        id);

    return Task.FromResult(record);
    }

    // Methods will go here

    public Task<EnrollmentRecord?> GetByIdAsync(string id)
    {
        _store.TryGetValue(id, out var record);
        return Task.FromResult(record);
    }

    
    public Task<IReadOnlyList<EnrollmentRecord>> GetAllAsync()
    {
        return Task.FromResult<IReadOnlyList<EnrollmentRecord>>(_store.Values.ToList());
    }

    public Task<bool> DeleteAsync(string id)
    {
    var removed = _store.Remove(id);
    return Task.FromResult(removed);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await Task.FromResult(_store.Remove(id));
    }

}

public record EnrollmentRecord(
    string Id,
    string StudentId,
    string CourseCode,
    DateTime EnrolledAt
);



