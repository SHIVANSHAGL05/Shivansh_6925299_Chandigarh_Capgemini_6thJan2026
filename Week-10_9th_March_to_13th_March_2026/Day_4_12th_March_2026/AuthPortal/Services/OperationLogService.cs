// Services/OperationLogService.cs
using AuthPortal.Models;

namespace AuthPortal.Services;

/// <summary>
/// Registered as Singleton — one shared instance for entire app lifetime.
/// Stores all CRUD operation logs in memory.
/// </summary>
public class OperationLogService : IOperationLogService
{
    private readonly List<OperationLog> _logs = new();
    private readonly object _lock = new();

    public void Log(string operation, string details, string section, long executionTimeMs)
    {
        lock (_lock)
        {
            _logs.Add(new OperationLog
            {
                Operation = operation,
                Details = details,
                Section = section,
                ExecutionTimeMs = executionTimeMs,
                Timestamp = DateTime.UtcNow
            });
        }
    }

    public IReadOnlyList<OperationLog> GetLogs()
    {
        lock (_lock)
        {
            return _logs.AsReadOnly();
        }
    }
}