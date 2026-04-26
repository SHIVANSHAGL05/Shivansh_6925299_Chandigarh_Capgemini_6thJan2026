using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LearningPlatform.Web.Models;
using LearningPlatform.Web.Services;

namespace LearningPlatform.Web.Pages.Student;

[Authorize]
public class MyEnrollmentsModel : PageModel
{
    private readonly IApiService _api;
    public MyEnrollmentsModel(IApiService api) => _api = api;

    public List<EnrollmentViewModel> Enrollments { get; set; } = new();

    public async Task OnGetAsync()
    {
        var token = HttpContext.Session.GetString("JwtToken");
        if (!string.IsNullOrEmpty(token))
            Enrollments = await _api.GetMyEnrollmentsAsync(token) ?? new();
    }
}
