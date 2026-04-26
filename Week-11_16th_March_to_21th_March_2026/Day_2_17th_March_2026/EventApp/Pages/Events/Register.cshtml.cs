using EventApp.Data;
using EventApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EventApp.Pages.Events
{
    /// <summary>
    /// Task 2: PageModel with OnGet() and OnPost() handlers.
    /// [BindProperty] binds form fields directly to the Registration property.
    /// </summary>
    public class RegisterModel : PageModel
    {
        // ── Task 2: [BindProperty] ────────────────────────────────────────
        // Razor Pages automatically maps posted form values to this property.
        [BindProperty]
        public EventRegistration Registration { get; set; } = new();

        // Confirmation message shown after successful registration
        public string? ConfirmationMessage { get; set; }

        // Dropdown list of available events
        public List<SelectListItem> AvailableEvents { get; set; } = new();

        // ── Task 2: OnGet() ───────────────────────────────────────────────
        // Called when the page is first loaded (GET /Events/Register)
        public void OnGet()
        {
            PopulateEventsList();
        }

        // ── Task 2: OnPost() ─────────────────────────────────────────────
        // Called when the form is submitted (POST /Events/Register)
        public IActionResult OnPost()
        {
            PopulateEventsList(); // always repopulate dropdown on postback

            // Task 3: Validate using data annotations — ModelState checks all [Required], [EmailAddress] etc.
            if (!ModelState.IsValid)
                return Page(); // re-display form with validation errors

            RegistrationStore.Add(Registration);

            // Pass success flag to the page via TempData (survives redirect)
            TempData["SuccessMessage"] = $"🎉 {Registration.ParticipantName} successfully registered for '{Registration.EventName}'!";
            return RedirectToPage("Index");
        }

        private void PopulateEventsList()
        {
            AvailableEvents = new List<SelectListItem>
            {
                new("Tech Summit 2025",        "Tech Summit 2025"),
                new("AI Workshop",             "AI Workshop"),
                new("Cloud Computing Bootcamp","Cloud Computing Bootcamp"),
                new("Web Dev Masterclass",     "Web Dev Masterclass"),
                new("Cybersecurity Forum",     "Cybersecurity Forum"),
            };
        }
    }
}
