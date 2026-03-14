// Models/AdminDashboardViewModel.cs
namespace AuthPortal.Models;

public class AdminDashboardViewModel
{
    public IEnumerable<Student> Students { get; set; } = new List<Student>();
    public IReadOnlyList<OperationLog> OperationLogs { get; set; } = new List<OperationLog>();
    public Student NewStudent { get; set; } = new();
    public string LoggedInUser { get; set; } = string.Empty;
}