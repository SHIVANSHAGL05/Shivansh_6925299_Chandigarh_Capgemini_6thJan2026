// Services/StudentService.cs
using AuthPortal.Models;

namespace AuthPortal.Services;

/// <summary>
/// Registered as Singleton — shares student data across all requests.
/// Static list simulates a database.
/// </summary>
public class StudentService : IStudentService
{
    private static List<Student> _students = new()
    {
        new Student { Id = 1, Name = "Alice Johnson", Email = "alice@example.com", Course = "Computer Science" },
        new Student { Id = 2, Name = "Bob Smith",     Email = "bob@example.com",   Course = "Mathematics"      },
        new Student { Id = 3, Name = "Carol White",   Email = "carol@example.com", Course = "Physics"          },
    };
    private static int _nextId = 4;

    public IEnumerable<Student> GetAll() => _students;

    public Student? GetById(int id) => _students.FirstOrDefault(s => s.Id == id);

    public void Add(Student student)
    {
        student.Id = _nextId++;
        _students.Add(student);
    }

    public void Update(Student student)
    {
        var existing = _students.FirstOrDefault(s => s.Id == student.Id);
        if (existing != null)
        {
            existing.Name = student.Name;
            existing.Email = student.Email;
            existing.Course = student.Course;
        }
    }

    public void Delete(int id)
    {
        var student = _students.FirstOrDefault(s => s.Id == id);
        if (student != null)
            _students.Remove(student);
    }
}