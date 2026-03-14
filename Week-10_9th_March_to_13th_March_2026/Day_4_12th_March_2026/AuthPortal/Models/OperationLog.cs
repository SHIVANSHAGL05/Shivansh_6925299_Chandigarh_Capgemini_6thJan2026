// Models/OperationLog.cs
namespace AuthPortal.Models;

public class OperationLog
{
    public string Operation { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public long ExecutionTimeMs { get; set; }
}