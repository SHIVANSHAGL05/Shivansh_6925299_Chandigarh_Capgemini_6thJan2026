using EmployeePortal.Models;

namespace EmployeePortal.Data
{
    /// <summary>
    /// EmployeeRepository — in-memory mock database.
    ///
    /// Registered as a SINGLETON in Program.cs, meaning one shared instance
    /// lives for the entire application lifetime. This makes the List<Employee>
    /// persist across all HTTP requests, simulating a real database.
    ///
    /// Replace with EF Core + SQL Server in a production application.
    /// </summary>
    public class EmployeeRepository
    {
        private static readonly List<Employee> _employees = new()
        {
            // Seed data — pre-populated so HR pages are not empty on first run.
            new Employee { Id = 1, Name = "Priya Sharma",   Age = 29, Email = "priya@company.com",  Department = "Engineering",    JoinedOn = DateTime.Now.AddMonths(-6) },
            new Employee { Id = 2, Name = "Rahul Verma",    Age = 34, Email = "rahul@company.com",  Department = "HR",             JoinedOn = DateTime.Now.AddMonths(-12) },
            new Employee { Id = 3, Name = "Anjali Mehta",   Age = 27, Email = "anjali@company.com", Department = "Finance",        JoinedOn = DateTime.Now.AddMonths(-3) },
            new Employee { Id = 4, Name = "Suresh Patel",   Age = 42, Email = "suresh@company.com", Department = "Operations",     JoinedOn = DateTime.Now.AddMonths(-18) },
            new Employee { Id = 5, Name = "Neha Gupta",     Age = 31, Email = "neha@company.com",   Department = "Marketing",      JoinedOn = DateTime.Now.AddMonths(-9) },
        };

        private static int _nextId = 6;

        public List<Employee> GetAll()               => _employees.ToList();
        public Employee? GetById(int id)             => _employees.FirstOrDefault(e => e.Id == id);
        public List<Employee> GetByDept(string dept) => _employees.Where(e => e.Department == dept).ToList();
        public int Count()                           => _employees.Count;

        public IEnumerable<string> GetDepartments() =>
            _employees.Select(e => e.Department!).Distinct().OrderBy(d => d);

        public Employee Add(Employee employee)
        {
            employee.Id       = _nextId++;
            employee.JoinedOn = DateTime.Now;
            _employees.Add(employee);
            return employee;
        }
    }
}
