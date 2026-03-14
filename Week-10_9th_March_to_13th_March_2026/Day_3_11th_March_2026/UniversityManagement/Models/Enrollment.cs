using System.ComponentModel.DataAnnotations;

namespace UniversityManagement.Models
{
    public enum Grade { A, B, C, D, F }

    public class Enrollment
    {
        public int EnrollmentId { get; set; }

        // FK to Student
        [Required]
        public int StudentId { get; set; }
        public Student? Student { get; set; }

        // FK to Course
        [Required]
        public int CourseId { get; set; }
        public Course? Course { get; set; }

        // Nullable: grade not assigned until course completed
        public Grade? Grade { get; set; }
    }
}
