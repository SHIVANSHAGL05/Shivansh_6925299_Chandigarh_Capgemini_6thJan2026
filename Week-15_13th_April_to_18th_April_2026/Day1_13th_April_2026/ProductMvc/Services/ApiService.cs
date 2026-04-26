using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ProductMvc.Models;

namespace ProductMvc.Services;

public class ApiService
{
    private readonly HttpClient _client;
    private readonly string _username;
    private readonly string _password;
    private string? _token;

    public ApiService(IHttpClientFactory factory, IConfiguration configuration)
    {
        _client = factory.CreateClient("ApiClient");
        _username = configuration["ApiAuth:Username"] ?? "admin";
        _password = configuration["ApiAuth:Password"] ?? "123";
    }

    public async Task<List<Product>> GetProductsAsync()
    {
        if (!await EnsureAuthenticatedAsync())
        {
            return [];
        }

        try
        {
            var response = await _client.GetAsync("api/product");
            if (!response.IsSuccessStatusCode)
            {
                return [];
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Product>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? [];
        }
        catch
        {
            return [];
        }
    }

    public async Task<(bool Success, string? ErrorMessage)> CreateProductAsync(Product product)
    {
        if (!await EnsureAuthenticatedAsync())
        {
            return (false, "Authentication failed.");
        }

        if (string.IsNullOrWhiteSpace(product.Name))
        {
            return (false, "Product name is required.");
        }

        if (product.Price <= 0)
        {
            return (false, "Price must be greater than 0.");
        }

        if (product.ImageFile is null || product.ImageFile.Length == 0)
        {
            return (false, "Product image is required.");
        }

        try
        {
            using var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(product.Name), nameof(product.Name));
            formData.Add(new StringContent(product.Price.ToString(System.Globalization.CultureInfo.InvariantCulture)), nameof(product.Price));

            await using var fileStream = product.ImageFile.OpenReadStream();
            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(product.ImageFile.ContentType);
            formData.Add(fileContent, "File", Path.GetFileName(product.ImageFile.FileName));

            var response = await _client.PostAsync("api/product", formData);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return (false, $"API error: {response.StatusCode}");
            }

            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, $"Request failed: {ex.Message}");
        }
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        if (!await EnsureAuthenticatedAsync())
        {
            return false;
        }

        var response = await _client.DeleteAsync($"api/product/{id}");
        // API returns 204 NoContent on success, which is considered success
        return response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.NoContent;
    }

    private async Task<bool> EnsureAuthenticatedAsync()
    {
        if (!string.IsNullOrWhiteSpace(_token))
        {
            return true;
        }

        var payload = JsonSerializer.Serialize(new
        {
            username = _username,
            password = _password
        });

        using var content = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("api/auth/login", content);
        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        var json = await response.Content.ReadAsStringAsync();
        var auth = JsonSerializer.Deserialize<AuthResponse>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (string.IsNullOrWhiteSpace(auth?.Token))
        {
            return false;
        }

        _token = auth.Token;
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        return true;
    }

    private sealed class AuthResponse
    {
        public string Token { get; set; } = string.Empty;
    }
}
