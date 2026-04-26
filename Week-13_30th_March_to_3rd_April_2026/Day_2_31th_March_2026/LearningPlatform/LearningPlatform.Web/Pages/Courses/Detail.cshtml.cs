using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LearningPlatform.Web.Models;
using LearningPlatform.Web.Services;

namespace LearningPlatform.Web.Pages.Courses;

public class DetailModel : PageModel
{
    private readonly IApiService _api;
    public DetailModel(IApiService api) => _api = api;

    public CourseViewModel?    Course         { get; set; }
    public List<LessonViewModel> Lessons      { get; set; } = new();
    public bool                AlreadyEnrolled { get; set; }
    public string?             EnrollMessage  { get; set; }
    public bool                EnrollSuccess  { get; set; }

    public async Task OnGetAsync(int id)
    {
        Course  = await _api.GetCourseAsync(id);
        Lessons = await _api.GetLessonsAsync(id) ?? new();

        // Check if already enrolled
        var token = HttpContext.Session.GetString("JwtToken");
        if (!string.IsNullOrEmpty(token))
        {
            var enrollments = await _api.GetMyEnrollmentsAsync(token);
            AlreadyEnrolled = enrollments?.Any(e => e.CourseId == id) ?? false;
        }
    }

    public async Task<IActionResult> OnPostAsync(int courseId)
    {
        var token = HttpContext.Session.GetString("JwtToken");
        if (string.IsNullOrEmpty(token))
            return RedirectToPage("/Auth/Login");

        var (success, error) = await _api.EnrollAsync(courseId, token);
        EnrollSuccess = success;
        EnrollMessage = success ? "✅ Enrolled successfully!" : error;

        Course  = await _api.GetCourseAsync(courseId);
        Lessons = await _api.GetLessonsAsync(courseId) ?? new();
        AlreadyEnrolled = success;

        return Page();
    }
}
