using System.ComponentModel.DataAnnotations;

namespace UniversityManagement.Models
{
    public class Student
    {
        public int StudentId { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [Display(Name = "Enrollment Date")]
        public DateTime EnrollmentDate { get; set; }

        // Navigation Property: One Student -> Many Enrollments
        public ICollection<Enrollment> Enrollments { get; set; }
        = new List<Enrollment>();
    }
}
