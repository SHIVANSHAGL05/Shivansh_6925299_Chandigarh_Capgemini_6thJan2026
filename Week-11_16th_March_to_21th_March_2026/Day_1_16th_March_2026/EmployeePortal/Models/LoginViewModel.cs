using System.ComponentModel.DataAnnotations;

namespace EmployeePortal.Models
{
    /// <summary>
    /// ViewModel for the Admin Login form.
    /// A ViewModel holds only the data a specific view needs —
    /// it is never stored in the database.
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
