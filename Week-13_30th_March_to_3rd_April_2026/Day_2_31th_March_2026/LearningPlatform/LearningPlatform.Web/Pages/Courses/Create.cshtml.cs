using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LearningPlatform.Web.Models;
using LearningPlatform.Web.Services;

namespace LearningPlatform.Web.Pages.Courses;

[Authorize(Roles = "Instructor,Admin")]
public class CreateModel : PageModel
{
    private readonly IApiService _api;
    public CreateModel(IApiService api) => _api = api;

    [BindProperty] public CreateCourseInputModel Input { get; set; } = new();
    public string? ErrorMessage { get; set; }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrWhiteSpace(Input.Title) || Input.Title.Length < 5)
            { ErrorMessage = "Title must be at least 5 characters."; return Page(); }
        if (string.IsNullOrWhiteSpace(Input.Description) || Input.Description.Length < 20)
            { ErrorMessage = "Description must be at least 20 characters."; return Page(); }

        var token = HttpContext.Session.GetString("JwtToken");
        if (string.IsNullOrEmpty(token))
            return RedirectToPage("/Auth/Login");

        var (success, error) = await _api.CreateCourseAsync(Input, token);
        if (!success)
        {
            ErrorMessage = error ?? "Failed to create course.";
            return Page();
        }

        return RedirectToPage("/Courses/Index");
    }
}
