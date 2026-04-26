using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using LearningPlatform.Web.Models;

namespace LearningPlatform.Web.Services;

public interface IApiService
{
    Task<AuthResponse?> LoginAsync(string email, string password);
    Task<(bool Success, string? Error)> RegisterAsync(RegisterInputModel model);
    Task<PagedResult<CourseViewModel>?> GetCoursesAsync(int page = 1, int pageSize = 9, string? search = null);
    Task<CourseViewModel?> GetCourseAsync(int id);
    Task<List<LessonViewModel>?> GetLessonsAsync(int courseId);
    Task<(bool Success, string? Error)> EnrollAsync(int courseId, string token);
    Task<List<EnrollmentViewModel>?> GetMyEnrollmentsAsync(string token);
    Task<(bool Success, string? Error)> CreateCourseAsync(CreateCourseInputModel model, string token);
}

public class ApiService : IApiService
{
    private readonly HttpClient _http;
    private static readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };

    public ApiService(HttpClient http) => _http = http;

    public async Task<AuthResponse?> LoginAsync(string email, string password)
    {
        var res = await _http.PostAsJsonAsync("api/auth/login", new { email, password });
        if (!res.IsSuccessStatusCode) return null;
        return await res.Content.ReadFromJsonAsync<AuthResponse>(_json);
    }

    public async Task<(bool Success, string? Error)> RegisterAsync(RegisterInputModel model)
    {
        var res = await _http.PostAsJsonAsync("api/auth/register", new
        {
            model.Username,
            model.Email,
            model.Password,
            model.Role
        });

        if (res.IsSuccessStatusCode) return (true, null);

        var err = await res.Content.ReadFromJsonAsync<ErrorResponse>(_json);
        return (false, err?.Error ?? "Registration failed");
    }

    public async Task<PagedResult<CourseViewModel>?> GetCoursesAsync(int page = 1, int pageSize = 9, string? search = null)
    {
        var url = $"api/v1/courses?page={page}&pageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(search)) url += $"&search={Uri.EscapeDataString(search)}";

        var res = await _http.GetAsync(url);
        if (!res.IsSuccessStatusCode) return null;
        return await res.Content.ReadFromJsonAsync<PagedResult<CourseViewModel>>(_json);
    }

    public async Task<CourseViewModel?> GetCourseAsync(int id)
    {
        var res = await _http.GetAsync($"api/v1/courses/{id}");
        if (!res.IsSuccessStatusCode) return null;
        return await res.Content.ReadFromJsonAsync<CourseViewModel>(_json);
    }

    public async Task<List<LessonViewModel>?> GetLessonsAsync(int courseId)
    {
        var res = await _http.GetAsync($"api/v1/courses/{courseId}/lessons");
        if (!res.IsSuccessStatusCode) return null;
        return await res.Content.ReadFromJsonAsync<List<LessonViewModel>>(_json);
    }

    public async Task<(bool Success, string? Error)> EnrollAsync(int courseId, string token)
    {
        using var req = new HttpRequestMessage(HttpMethod.Post, "api/v1/enroll");
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        req.Content = new StringContent(
            JsonSerializer.Serialize(new { courseId }),
            Encoding.UTF8, "application/json");

        var res = await _http.SendAsync(req);
        if (res.IsSuccessStatusCode) return (true, null);

        var err = await res.Content.ReadFromJsonAsync<ErrorResponse>(_json);
        return (false, err?.Error ?? "Enrollment failed");
    }

    public async Task<List<EnrollmentViewModel>?> GetMyEnrollmentsAsync(string token)
    {
        using var req = new HttpRequestMessage(HttpMethod.Get, "api/v1/enroll/my");
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var res = await _http.SendAsync(req);
        if (!res.IsSuccessStatusCode) return null;
        return await res.Content.ReadFromJsonAsync<List<EnrollmentViewModel>>(_json);
    }

    public async Task<(bool Success, string? Error)> CreateCourseAsync(CreateCourseInputModel model, string token)
    {
        using var req = new HttpRequestMessage(HttpMethod.Post, "api/v1/courses");
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        req.Content = new StringContent(
            JsonSerializer.Serialize(new
            {
                model.Title,
                model.Description,
                model.Category,
                model.Price,
                model.Level
            }),
            Encoding.UTF8, "application/json");

        var res = await _http.SendAsync(req);
        if (res.IsSuccessStatusCode) return (true, null);

        var err = await res.Content.ReadFromJsonAsync<ErrorResponse>(_json);
        return (false, err?.Error ?? "Failed to create course");
    }

    private class ErrorResponse
    {
        public string? Error { get; set; }
    }
}
