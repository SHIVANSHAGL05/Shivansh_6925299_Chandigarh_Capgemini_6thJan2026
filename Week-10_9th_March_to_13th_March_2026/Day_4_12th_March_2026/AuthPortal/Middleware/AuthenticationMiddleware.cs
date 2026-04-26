// Middleware/AuthenticationMiddleware.cs
using AuthPortal.Services;

namespace AuthPortal.Middleware;

/// <summary>
/// Custom middleware that protects all /Admin/* routes.
/// IAuthService is injected via DI to check authentication.
/// Public routes like /Students/* pass through freely.
/// </summary>
public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    public AuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IAuthService authService)
    {
        var path = context.Request.Path.Value ?? "";

        // Protect all /Admin/* routes except /Admin/Login itself
        if (path.StartsWith("/Admin", StringComparison.OrdinalIgnoreCase)
            && !path.Equals("/Admin/Login", StringComparison.OrdinalIgnoreCase))
        {
            if (!authService.IsAuthenticated(context))
            {
                // Redirect unauthenticated users to login page
                context.Response.Redirect("/Admin/Login");
                return;
            }
        }

        // Allow all public routes and authenticated admin routes
        await _next(context);
    }
}