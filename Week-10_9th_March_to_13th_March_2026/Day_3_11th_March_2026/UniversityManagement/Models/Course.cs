using System.ComponentModel.DataAnnotations;

namespace UniversityManagement.Models
{
    public class Course
    {
        public int CourseId { get; set; }

        [Required, StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Range(1, 6)]
        public int Credits { get; set; }

        // FK - Optional: An instructor teaches this course
        public int? InstructorId { get; set; }
        public Instructor? Instructor { get; set; }

        // Navigation Property: One Course -> Many Enrollments
        public ICollection<Enrollment> Enrollments { get; set; }
        = new List<Enrollment>();
    }
}
