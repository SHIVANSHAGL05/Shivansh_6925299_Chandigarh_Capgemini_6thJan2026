using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LearningPlatform.Web.Models;
using LearningPlatform.Web.Services;

namespace LearningPlatform.Web.Pages.Courses;

public class IndexModel : PageModel
{
    private readonly IApiService _api;
    public IndexModel(IApiService api) => _api = api;

    public List<CourseViewModel> Courses { get; set; } = new();
    public int CurrentPage { get; set; } = 1;
    public int TotalPages  { get; set; } = 1;
    public string? Search   { get; set; }
    public string? Category { get; set; }

    public async Task OnGetAsync(int page = 1, string? search = null, string? category = null)
    {
        Search   = search;
        Category = category;
        CurrentPage = page;

        var result = await _api.GetCoursesAsync(page, 9, search);
        if (result != null)
        {
            var items = result.Items.ToList();

            // Client-side category filter (API doesn't have category param)
            if (!string.IsNullOrWhiteSpace(category))
                items = items.Where(c => c.Category == category).ToList();

            Courses    = items;
            TotalPages = result.TotalPages;
        }
    }
}
