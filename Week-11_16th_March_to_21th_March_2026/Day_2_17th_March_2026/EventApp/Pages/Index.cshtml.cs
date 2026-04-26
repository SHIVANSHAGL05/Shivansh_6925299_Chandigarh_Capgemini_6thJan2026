using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace EventApp.Pages
{
    public class IndexModel : PageModel
    {
        public string? StatusMessage { get; set; }

        // ── GET ───────────────────────────────────────────────────────────
        // Task 7 (demo): loginAsAdmin=true signs in a fake Admin claim so
        // User.IsInRole("Admin") returns true — simulates authentication
        // without needing a full Identity setup.
        public async Task<IActionResult> OnGetAsync(bool loginAsAdmin = false, bool logout = false)
        {
            if (logout)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToPage();
            }

            if (loginAsAdmin)
            {
                // Build a ClaimsPrincipal with the "Admin" role claim
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "Admin User"),
                    new Claim(ClaimTypes.Role, "Admin"),
                };
                var identity  = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal);

                TempData["SuccessMessage"] = "✅ Logged in as Admin. You can now delete participants.";
                return RedirectToPage("/Events/Index");
            }

            return Page();
        }
    }
}
