using EventApp.Data;
using EventApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EventApp.Pages.Events
{
    /// <summary>
    /// Task 4: Index PageModel — fetches participant list in OnGet().
    /// Task 5: OnPostDelete(int id) removes a participant by ID.
    /// Task 7: Role check ensures only Admin can delete.
    /// </summary>
    public class IndexModel : PageModel
    {
        // Task 4: Populated by OnGet(), bound to the view
        public List<EventRegistration> Registrations { get; set; } = new();

        // Filter state (optional UX enhancement — persists across postback via [BindProperty])
        [BindProperty(SupportsGet = true)]
        public string? FilterEvent { get; set; }

        // ── Task 4: OnGet() ───────────────────────────────────────────────
        // Runs on every GET — fetches (and optionally filters) the participant list
        public void OnGet()
        {
            Registrations = string.IsNullOrWhiteSpace(FilterEvent)
                ? RegistrationStore.Registrations.OrderByDescending(r => r.RegisteredAt).ToList()
                : RegistrationStore.Registrations
                    .Where(r => r.EventName == FilterEvent)
                    .OrderByDescending(r => r.RegisteredAt)
                    .ToList();
        }

        // ── Task 5: OnPostDelete(int id) ──────────────────────────────────
        // Named handler: triggered by <form asp-page-handler="Delete" ...>
        // URL: POST /Events/Index?handler=Delete
        public IActionResult OnPostDelete(int id)
        {
            // ── Task 7: Role check — only Admin may delete ────────────────
            // User.IsInRole() reads from the ClaimsPrincipal set by authentication middleware.
            // In production wire up ASP.NET Core Identity; for this demo we simulate with a
            // query-string flag (?isAdmin=true) via a custom claim added in Program.cs.
            if (!User.IsInRole("Admin"))
            {
                TempData["ErrorMessage"] = "⛔ Access denied. Only Admins can remove participants.";
                return RedirectToPage();
            }

            var deleted = RegistrationStore.Delete(id);
            TempData["SuccessMessage"] = deleted
                ? "✅ Participant removed successfully."
                : "⚠️ Participant not found.";

            return RedirectToPage();
        }
    }
}
