// Services/IOperationLogService.cs
using AuthPortal.Models;

namespace AuthPortal.Services;

public interface IOperationLogService
{
    void Log(string operation, string details, string section, long executionTimeMs);
    IReadOnlyList<OperationLog> GetLogs();
}