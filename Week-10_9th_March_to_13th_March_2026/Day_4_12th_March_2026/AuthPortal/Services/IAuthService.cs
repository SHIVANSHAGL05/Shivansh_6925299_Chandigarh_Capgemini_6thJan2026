// Services/IAuthService.cs
namespace AuthPortal.Services;

public interface IAuthService
{
    bool IsAuthenticated(HttpContext context);
    bool ValidateCredentials(string username, string password);
    void SignIn(HttpContext context, string username);
    void SignOut(HttpContext context);
}