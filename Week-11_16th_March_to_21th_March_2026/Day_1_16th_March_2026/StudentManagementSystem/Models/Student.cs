using System.ComponentModel.DataAnnotations;

namespace StudentManagementSystem.Models
{
    /// <summary>
    /// Student entity — Data Annotations serve two purposes:
    ///   1. EF Core uses them to set database column constraints.
    ///   2. ASP.NET Core MVC uses them for server + client-side validation.
    /// </summary>
    public class Student
    {
        // Primary key — EF Core recognises "Id" naming convention automatically.
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "Name must be between 2 and 100 characters.")]
        [Display(Name = "Full Name")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Age is required.")]
        [Range(18, 60, ErrorMessage = "Age must be between 18 and 60.")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(150)]
        [Display(Name = "Email Address")]
        public string? Email { get; set; }
    }
}
