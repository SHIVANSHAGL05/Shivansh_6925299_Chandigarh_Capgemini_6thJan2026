using StudentManagementSystem.Models;

namespace StudentManagementSystem.Data
{
    /// <summary>
    /// StudentRepository acts as an in-memory mock database.
    ///
    /// WHY STATIC?
    ///   A static list persists for the lifetime of the application process.
    ///   This means data survives across multiple HTTP requests — simulating
    ///   what a real database would do.
    ///
    ///   In a real application you would replace this with EF Core + SQL Server.
    ///
    /// Registered in Program.cs as a Singleton so the same instance
    /// (and the same list) is shared across all controller requests.
    /// </summary>
    public class StudentRepository
    {
        private static readonly List<Student> _students = new();
        private static int _nextId = 1;

        /// <summary>Returns all registered students.</summary>
        public List<Student> GetAll() => _students.ToList();

        /// <summary>Finds a student by their ID. Returns null if not found.</summary>
        public Student? GetById(int id) =>
            _students.FirstOrDefault(s => s.Id == id);

        /// <summary>
        /// Adds a new student, assigns a unique auto-incremented ID,
        /// and returns the saved student (with Id populated).
        /// </summary>
        public Student Add(Student student)
        {
            student.Id = _nextId++;
            _students.Add(student);
            return student;
        }

        /// <summary>Returns the total number of registered students.</summary>
        public int Count() => _students.Count;
    }
}
