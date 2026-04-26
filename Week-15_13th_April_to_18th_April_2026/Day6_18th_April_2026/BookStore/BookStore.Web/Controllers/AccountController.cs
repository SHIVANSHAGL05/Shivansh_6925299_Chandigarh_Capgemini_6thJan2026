using System.Security.Claims;
using BookStore.Web.Models.ViewModels;
using BookStore.Web.Services.Auth;
using BookStore.Web.Services.Commerce;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Web.Controllers;

public class AccountController : Controller
{
    private readonly IAuthApiClient _authApiClient;
    private readonly ICommerceApiClient _commerceApiClient;

    public AccountController(IAuthApiClient authApiClient, ICommerceApiClient commerceApiClient)
    {
        _authApiClient = authApiClient;
        _commerceApiClient = commerceApiClient;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Portal", "Home");
        }

        return View(new LoginViewModel());
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _authApiClient.LoginAsync(model, cancellationToken);
        if (!result.Success || result.Payload is null)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        await SignInAsync(result.Payload, model.RememberMe);
        TempData["AuthMessage"] = "Login successful.";
        return RedirectToAction("Portal", "Home");
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Portal", "Home");
        }

        return View(new RegisterViewModel());
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (string.Equals(model.Role, "Admin", StringComparison.OrdinalIgnoreCase) && string.IsNullOrWhiteSpace(model.AdminRegistrationKey))
        {
            ModelState.AddModelError(nameof(model.AdminRegistrationKey), "Admin registration key is required for admin signup.");
            return View(model);
        }

        var result = await _authApiClient.RegisterAsync(model, cancellationToken);
        if (!result.Success || result.Payload is null)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        await SignInAsync(result.Payload, false);

        TempData["AuthMessage"] = string.Equals(result.Payload.Role, "Admin", StringComparison.OrdinalIgnoreCase)
            ? "Admin account created and signed in."
            : "User account created and signed in.";

        return RedirectToAction("Portal", "Home");
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        TempData["AuthMessage"] = "You have been signed out.";
        return RedirectToAction("Index", "Home");
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Profile(CancellationToken cancellationToken)
    {
        var accessToken = User.FindFirst("access_token")?.Value;
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            TempData["AuthMessage"] = "Session expired. Please login again.";
            return RedirectToAction(nameof(Login));
        }

        var result = await _commerceApiClient.GetProfileAsync(accessToken, cancellationToken);
        if (!result.Success || result.Data is null)
        {
            TempData["AuthMessage"] = result.Message;
            return View(new ProfileViewModel());
        }

        var model = new ProfileViewModel
        {
            FullName = result.Data.FullName,
            Email = result.Data.Email,
            Phone = result.Data.Phone,
            Address = result.Data.Address,
            City = result.Data.City,
            Pincode = result.Data.Pincode
        };

        return View(model);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(ProfileViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var accessToken = User.FindFirst("access_token")?.Value;
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            TempData["AuthMessage"] = "Session expired. Please login again.";
            return RedirectToAction(nameof(Login));
        }

        var request = new UpdateProfileRequest
        {
            FullName = model.FullName,
            Phone = model.Phone,
            Address = model.Address,
            City = model.City,
            Pincode = model.Pincode
        };

        var result = await _commerceApiClient.UpdateProfileAsync(request, accessToken, cancellationToken);
        TempData["AuthMessage"] = result.Success ? "Profile updated." : result.Message;

        return RedirectToAction(nameof(Profile));
    }

    private async Task SignInAsync(ApiAuthPayload payload, bool rememberMe)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, payload.FullName),
            new(ClaimTypes.Email, payload.Email),
            new(ClaimTypes.Role, payload.Role),
            new("access_token", payload.Token),
            new("refresh_token", payload.RefreshToken)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = rememberMe,
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(12)
        };

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);
    }
}
