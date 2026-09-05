namespace TmsApi.Application.DTOs;

public record CreateCourseRequest(
    string Code,
    string Title,
    int MaxCapacity);
