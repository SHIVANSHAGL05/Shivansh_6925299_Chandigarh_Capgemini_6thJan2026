using System.ComponentModel.DataAnnotations;

namespace UniversityManagement.Models
{
    public class Instructor
    {
        public int InstructorId { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        // FK: Each instructor belongs to one Department
        public int DepartmentId { get; set; }
        public Department? Department { get; set; }

        // Navigation Property: One Instructor -> Many Courses
        public ICollection<Course> Courses { get; set; }
        = new List<Course>();
    }
}
