using System.ComponentModel.DataAnnotations;

namespace StudentRegistration.Models
{
    /// <summary>
    /// Student entity.
    /// - EF Core uses these annotations for column constraints (MaxLength, Required).
    /// - ASP.NET Core MVC uses them for server-side AND client-side validation.
    /// </summary>
    public class Student
    {
        // Primary key — EF Core recognises the "Id" naming convention automatically.
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "Name must be between 2 and 100 characters.")]
        [Display(Name = "Full Name")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Age is required.")]
        [Range(18, 60, ErrorMessage = "Age must be between 18 and 60.")]
        public int Age { get; set; }
    }
}
