using System.ComponentModel.DataAnnotations;

namespace EventApp.Models
{
    /// <summary>Task 1: EventRegistration model with data annotations for validation.</summary>
    public class EventRegistration
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Participant name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be 2–100 characters.")]
        [Display(Name = "Participant Name")]
        public string ParticipantName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Event name is required.")]
        [StringLength(200)]
        [Display(Name = "Event Name")]
        public string EventName { get; set; } = string.Empty;

        // Extra: timestamp for display in participant list
        public DateTime RegisteredAt { get; set; } = DateTime.Now;
    }
}
