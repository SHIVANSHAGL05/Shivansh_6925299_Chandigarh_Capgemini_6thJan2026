using System;
using System.Collections.Generic;
using System.Linq;
using StudentManagement.Models;

namespace StudentManagement.Services
{
    // Simple in-memory store — swap for DbContext/EF Core in production
    public interface IStudentService
    {
        IEnumerable<Student> GetAll();
        Student GetById(int id);
        void Add(Student student);
        void Update(Student student);
        void Delete(int id);
    }

    public class StudentService : IStudentService
    {
        private static List<Student> _students = new List<Student>
        {
            new Student { Id = 1, Name = "Aarav Sharma",  Email = "aarav@example.com",  Course = "Full Stack Development", JoiningDate = new DateTime(2024, 1, 15) },
            new Student { Id = 2, Name = "Priya Mehta",   Email = "priya@example.com",  Course = "Data Science",           JoiningDate = new DateTime(2024, 3, 10) },
            new Student { Id = 3, Name = "Rohan Verma",   Email = "rohan@example.com",  Course = "Cloud Computing",        JoiningDate = new DateTime(2024, 6, 20) },
            new Student { Id = 4, Name = "Neha Gupta",    Email = "neha@example.com",   Course = "UI/UX Design",           JoiningDate = new DateTime(2025, 1, 5)  },
            new Student { Id = 5, Name = "Karan Patel",   Email = "karan@example.com",  Course = "Cybersecurity",          JoiningDate = new DateTime(2025, 2, 18) },
        };

        private static int _nextId = 6;

        public IEnumerable<Student> GetAll() => _students.OrderBy(s => s.Name);

        public Student GetById(int id) => _students.FirstOrDefault(s => s.Id == id);

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
                existing.Name        = student.Name;
                existing.Email       = student.Email;
                existing.Course      = student.Course;
                existing.JoiningDate = student.JoiningDate;
            }
        }

        public void Delete(int id) => _students.RemoveAll(s => s.Id == id);
    }
}
