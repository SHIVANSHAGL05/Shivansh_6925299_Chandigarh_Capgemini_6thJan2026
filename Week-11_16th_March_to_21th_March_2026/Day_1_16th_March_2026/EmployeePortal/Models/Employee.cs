using System.ComponentModel.DataAnnotations;

namespace EmployeePortal.Models
{
    /// <summary>
    /// Employee entity.
    /// Data Annotations serve two purposes:
    ///   1. ASP.NET Core MVC — server-side AND client-side validation
    ///   2. Documentation    — communicates business rules to readers
    /// </summary>
    public class Employee
    {
        // Auto-incremented by EmployeeRepository when saved.
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "Name must be between 2 and 100 characters.")]
        [Display(Name = "Full Name")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Age is required.")]
        [Range(21, 65, ErrorMessage = "Age must be between 21 and 65.")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(150)]
        [Display(Name = "Email Address")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Department is required.")]
        [StringLength(100)]
        public string? Department { get; set; }

        // Automatically set to the registration date/time when saved.
        public DateTime JoinedOn { get; set; } = DateTime.Now;
    }
}
