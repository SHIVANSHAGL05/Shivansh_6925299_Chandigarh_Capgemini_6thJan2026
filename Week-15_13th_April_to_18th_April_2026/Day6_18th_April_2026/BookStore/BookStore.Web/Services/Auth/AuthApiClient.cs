using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using BookStore.Web.Models.ViewModels;

namespace BookStore.Web.Services.Auth;

public sealed class AuthApiClient : IAuthApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;

    public AuthApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<AuthResult> LoginAsync(LoginViewModel model, CancellationToken cancellationToken = default)
    {
        var payload = new
        {
            model.Email,
            model.Password
        };

        return await SendAuthRequestAsync("api/v1/auth/login", payload, cancellationToken);
    }

    public async Task<AuthResult> RegisterAsync(RegisterViewModel model, CancellationToken cancellationToken = default)
    {
        var payload = new RegisterRequest
        {
            FullName = model.FullName,
            Email = model.Email,
            Password = model.Password,
            Phone = model.Phone,
            Role = model.Role,
            Address = model.Address,
            City = model.City,
            Pincode = model.Pincode,
            AdminRegistrationKey = string.IsNullOrWhiteSpace(model.AdminRegistrationKey) ? null : model.AdminRegistrationKey
        };

        return await SendAuthRequestAsync("api/v1/auth/register", payload, cancellationToken);
    }

    private async Task<AuthResult> SendAuthRequestAsync(string endpoint, object payload, CancellationToken cancellationToken)
    {
        using var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        try
        {
            using var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);
            var raw = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var message = ExtractErrorMessage(raw);
                return new AuthResult
                {
                    Success = false,
                    Message = string.IsNullOrWhiteSpace(message) ? "Authentication request failed." : message
                };
            }

            var envelope = JsonSerializer.Deserialize<ApiAuthResponseEnvelope>(raw, JsonOptions);
            if (envelope?.Data is null || string.IsNullOrWhiteSpace(envelope.Data.Token))
            {
                return new AuthResult { Success = false, Message = "Authentication response was empty." };
            }

            return new AuthResult
            {
                Success = true,
                Message = string.IsNullOrWhiteSpace(envelope.Message) ? "Success" : envelope.Message,
                Payload = envelope.Data
            };
        }
        catch
        {
            return new AuthResult
            {
                Success = false,
                Message = "Unable to reach API. Ensure BookStore.API is running and reachable."
            };
        }
    }

    private static string ExtractErrorMessage(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return string.Empty;
        }

        try
        {
            using var doc = JsonDocument.Parse(raw);
            var root = doc.RootElement;

            if (root.TryGetProperty("message", out var messageProp) && messageProp.ValueKind == JsonValueKind.String)
            {
                return messageProp.GetString() ?? string.Empty;
            }

            if (root.TryGetProperty("errors", out var errorsProp) && errorsProp.ValueKind == JsonValueKind.Array)
            {
                var items = errorsProp.EnumerateArray()
                    .Where(x => x.ValueKind == JsonValueKind.String)
                    .Select(x => x.GetString())
                    .Where(x => !string.IsNullOrWhiteSpace(x));

                return string.Join(" ", items!);
            }
        }
        catch
        {
            return string.Empty;
        }

        return string.Empty;
    }
}
