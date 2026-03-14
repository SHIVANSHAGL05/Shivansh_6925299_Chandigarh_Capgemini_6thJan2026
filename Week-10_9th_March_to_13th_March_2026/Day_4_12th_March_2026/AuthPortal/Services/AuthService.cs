// Services/AuthService.cs
namespace AuthPortal.Services;

/// <summary>
/// Handles authentication via session.
/// Registered as Scoped in DI — one instance per HTTP request.
/// </summary>
public class AuthService : IAuthService
{
    // Simulated user store — replace with DB in production
    private static readonly Dictionary<string, string> _validUsers = new()
    {
        { "admin",     "admin123"  },
        { "superuser", "pass@word1" }
    };

    public bool IsAuthenticated(HttpContext context)
    {
        return context.Session.GetString("IsAuthenticated") == "true";
    }

    public bool ValidateCredentials(string username, string password)
    {
        return _validUsers.TryGetValue(username, out var stored)
               && stored == password;
    }

    public void SignIn(HttpContext context, string username)
    {
        context.Session.SetString("IsAuthenticated", "true");
        context.Session.SetString("Username", username);
    }

    public void SignOut(HttpContext context)
    {
        context.Session.Clear();
    }
}