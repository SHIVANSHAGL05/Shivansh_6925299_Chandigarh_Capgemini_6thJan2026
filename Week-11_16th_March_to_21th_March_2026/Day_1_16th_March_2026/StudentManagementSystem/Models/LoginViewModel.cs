using System.ComponentModel.DataAnnotations;

namespace StudentManagementSystem.Models
{
    /// <summary>
    /// ViewModel used only for the Login form — not stored in the database.
    /// A ViewModel holds only the data a specific view needs.
    /// </summary>
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username is required.")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
