using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LearningPlatform.Web.Models;
using LearningPlatform.Web.Services;

namespace LearningPlatform.Web.Pages.Auth;

public class LoginModel : PageModel
{
    private readonly IApiService _api;
    public LoginModel(IApiService api) => _api = api;

    [BindProperty] public LoginInputModel Input { get; set; } = new();
    public string? ErrorMessage { get; set; }

    public IActionResult OnGet()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToPage("/Courses/Index");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrWhiteSpace(Input.Email))   { ErrorMessage = "Email is required.";    return Page(); }
        if (string.IsNullOrWhiteSpace(Input.Password)) { ErrorMessage = "Password is required."; return Page(); }

        var auth = await _api.LoginAsync(Input.Email, Input.Password);
        if (auth == null)
        {
            ErrorMessage = "Invalid email or password.";
            return Page();
        }

        // Store JWT in session so page models can use it for API calls
        HttpContext.Session.SetString("JwtToken",  auth.Token);
        HttpContext.Session.SetString("RefreshToken", auth.RefreshToken);
        HttpContext.Session.SetString("Role", auth.Role);

        // Sign in with cookie so [Authorize] works on pages
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name,  auth.Username),
            new(ClaimTypes.Email, auth.Username),
            new(ClaimTypes.Role,  auth.Role),
            new("Role", auth.Role),
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)),
            new AuthenticationProperties { IsPersistent = true, ExpiresUtc = auth.Expiry });

        return RedirectToPage("/Courses/Index");
    }
}
