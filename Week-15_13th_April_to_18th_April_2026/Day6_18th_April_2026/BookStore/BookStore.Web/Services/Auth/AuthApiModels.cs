namespace BookStore.Web.Services.Auth;

public sealed class ApiAuthResponseEnvelope
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public ApiAuthPayload? Data { get; init; }
}

public sealed class ApiAuthPayload
{
    public string Token { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
}

public sealed class AuthResult
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public ApiAuthPayload? Payload { get; init; }
}

public sealed class RegisterRequest
{
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string Role { get; init; } = "Customer";
    public string? AdminRegistrationKey { get; init; }
    public string Address { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string Pincode { get; init; } = string.Empty;
}
