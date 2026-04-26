using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using SmartHealthcare.Core.Common;

namespace SmartHealthcare.MVC.Services;

public interface IApiService
{
    Task<ApiResponse<T>?> GetAsync<T>(string endpoint);
    Task<ApiResponse<T>?> PostAsync<T>(string endpoint, object payload);
    Task<ApiResponse<T>?> PutAsync<T>(string endpoint, object payload);
    Task<ApiResponse<T>?> PatchAsync<T>(string endpoint, object payload);
    Task<ApiResponse<T>?> DeleteAsync<T>(string endpoint);
}

public class ApiService : IApiService
{
    private readonly IHttpClientFactory _factory;
    private readonly IHttpContextAccessor _ctx;
    private readonly ILogger<ApiService> _logger;

    private static readonly JsonSerializerOptions _jsonOpts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ApiService(IHttpClientFactory factory, IHttpContextAccessor ctx, ILogger<ApiService> logger)
    {
        _factory = factory;
        _ctx     = ctx;
        _logger  = logger;
    }

    public Task<ApiResponse<T>?> GetAsync<T>(string endpoint)      => SendAsync<T>(HttpMethod.Get,    endpoint);
    public Task<ApiResponse<T>?> PostAsync<T>(string endpoint, object p)   => SendAsync<T>(HttpMethod.Post,   endpoint, p);
    public Task<ApiResponse<T>?> PutAsync<T>(string endpoint, object p)    => SendAsync<T>(HttpMethod.Put,    endpoint, p);
    public Task<ApiResponse<T>?> PatchAsync<T>(string endpoint, object p)  => SendAsync<T>(new HttpMethod("PATCH"), endpoint, p);
    public Task<ApiResponse<T>?> DeleteAsync<T>(string endpoint)   => SendAsync<T>(HttpMethod.Delete,  endpoint);

    private async Task<ApiResponse<T>?> SendAsync<T>(HttpMethod method, string endpoint, object? payload = null)
    {
        try
        {
            var client  = _factory.CreateClient("HealthcareAPI");
            var token   = _ctx.HttpContext?.Session.GetString("JwtToken");

            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpContent? content = null;
            if (payload != null)
                content = new StringContent(
                    JsonSerializer.Serialize(payload),
                    Encoding.UTF8,
                    "application/json");

            var request  = new HttpRequestMessage(method, endpoint) { Content = content };
            var response = await client.SendAsync(request);
            var json     = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<ApiResponse<T>>(json, _jsonOpts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "API call failed: {Method} {Endpoint}", method, endpoint);
            return null;
        }
    }
}
