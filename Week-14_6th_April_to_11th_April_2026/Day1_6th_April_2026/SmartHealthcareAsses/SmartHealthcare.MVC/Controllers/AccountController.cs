using Microsoft.AspNetCore.Mvc;
using SmartHealthcare.Core.DTOs;
using SmartHealthcare.MVC.Models;
using SmartHealthcare.MVC.Services;

namespace SmartHealthcare.MVC.Controllers;

public class AccountController : Controller
{
    private readonly IApiService _api;
    private readonly ILogger<AccountController> _logger;

    public AccountController(IApiService api, ILogger<AccountController> logger)
    {
        _api    = api;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (HttpContext.Session.GetString("JwtToken") != null)
            return RedirectToAction("Index", "Home");

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _api.PostAsync<AuthResponseDto>("api/auth/login", new LoginDto
        {
            Email    = model.Email,
            Password = model.Password
        });

        if (result == null || !result.Success)
        {
            ModelState.AddModelError(string.Empty, result?.Message ?? "Login failed. Please try again.");
            _logger.LogWarning("Login failed for {Email}", model.Email);
            return View(model);
        }

        // Store token and user info in session
        HttpContext.Session.SetString("JwtToken",     result.Data!.AccessToken);
        HttpContext.Session.SetString("RefreshToken", result.Data.RefreshToken);
        HttpContext.Session.SetString("UserEmail",    result.Data.Email);
        HttpContext.Session.SetString("UserName",     result.Data.FullName);
        HttpContext.Session.SetString("UserRole",     result.Data.Roles.FirstOrDefault() ?? "Patient");
        HttpContext.Session.SetInt32("UserId",        result.Data.UserId);

        _logger.LogInformation("User {Email} logged in successfully", model.Email);

        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            return Redirect(model.ReturnUrl);

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _api.PostAsync<AuthResponseDto>("api/auth/register", new RegisterDto
        {
            FirstName       = model.FirstName,
            LastName        = model.LastName,
            Email           = model.Email,
            Password        = model.Password,
            ConfirmPassword = model.ConfirmPassword,
            Role            = model.Role,
            PhoneNumber     = model.PhoneNumber,
            DateOfBirth     = model.DateOfBirth,
            Gender          = model.Gender
        });

        if (result == null || !result.Success)
        {
            foreach (var err in result?.Errors ?? new())
                ModelState.AddModelError(string.Empty, err);
            if (!result?.Errors.Any() ?? true)
                ModelState.AddModelError(string.Empty, result?.Message ?? "Registration failed.");
            return View(model);
        }

        TempData["Success"] = "Registration successful! Please log in.";
        return RedirectToAction(nameof(Login));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = HttpContext.Session.GetString("RefreshToken");
        if (!string.IsNullOrEmpty(refreshToken))
            await _api.PostAsync<bool>("api/auth/revoke-token", new { refreshToken });

        HttpContext.Session.Clear();
        _logger.LogInformation("User logged out");
        return RedirectToAction(nameof(Login));
    }

    public IActionResult AccessDenied() => View();
}
