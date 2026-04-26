using StudentPortal.Models;
using StudentPortal.Services;

namespace StudentPortal.ViewModels
{
    public class StudentIndexViewModel
    {
        public IEnumerable<Student>           Students    { get; set; } = new List<Student>();
        public IReadOnlyList<RequestLogEntry> RequestLogs { get; set; } = new List<RequestLogEntry>();
    }
}
