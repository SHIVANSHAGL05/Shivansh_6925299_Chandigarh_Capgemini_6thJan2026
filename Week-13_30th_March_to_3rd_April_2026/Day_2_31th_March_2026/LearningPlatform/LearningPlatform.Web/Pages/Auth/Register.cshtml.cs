using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LearningPlatform.Web.Models;
using LearningPlatform.Web.Services;

namespace LearningPlatform.Web.Pages.Auth;

public class RegisterModel : PageModel
{
    private readonly IApiService _api;
    public RegisterModel(IApiService api) => _api = api;

    [BindProperty] public RegisterInputModel Input { get; set; } = new();
    public string? ErrorMessage   { get; set; }
    public string? SuccessMessage { get; set; }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrWhiteSpace(Input.Username) || Input.Username.Length < 3)
            { ErrorMessage = "Username must be at least 3 characters."; return Page(); }
        if (string.IsNullOrWhiteSpace(Input.Email))
            { ErrorMessage = "Email is required."; return Page(); }
        if (string.IsNullOrWhiteSpace(Input.Password) || Input.Password.Length < 6)
            { ErrorMessage = "Password must be at least 6 characters."; return Page(); }
        if (Input.Password != Input.ConfirmPassword)
            { ErrorMessage = "Passwords do not match."; return Page(); }

        var (success, error) = await _api.RegisterAsync(Input);
        if (!success)
        {
            ErrorMessage = error ?? "Registration failed.";
            return Page();
        }

        SuccessMessage = "Account created! Redirecting to login...";
        return RedirectToPage("/Auth/Login");
    }
}
