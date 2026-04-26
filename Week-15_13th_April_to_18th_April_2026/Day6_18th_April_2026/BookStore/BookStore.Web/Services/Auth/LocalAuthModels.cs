namespace BookStore.Web.Services.Auth;

public sealed class LocalSessionUser
{
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Role { get; init; } = "Customer";
}

public sealed class LocalAuthResult
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public LocalSessionUser? User { get; init; }

    public static LocalAuthResult Ok(LocalSessionUser user) =>
        new() { Success = true, Message = "Success", User = user };

    public static LocalAuthResult Fail(string message) =>
        new() { Success = false, Message = message };
}
