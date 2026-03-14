// Services/IStudentService.cs
using AuthPortal.Models;

namespace AuthPortal.Services;

public interface IStudentService
{
    IEnumerable<Student> GetAll();
    Student? GetById(int id);
    void Add(Student student);
    void Update(Student student);
    void Delete(int id);
}