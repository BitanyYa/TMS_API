namespace TmsApi.Infrastructure.Services;

public interface IAuditService
{
    void Record(string message);

    IReadOnlyList<string> Entries { get; }
}

public class AuditService : IAuditService
{
    private readonly List<string> _entries = new();

    public IReadOnlyList<string> Entries => _entries;

    public void Record(string message)
    {
        _entries.Add(message);
    }
}