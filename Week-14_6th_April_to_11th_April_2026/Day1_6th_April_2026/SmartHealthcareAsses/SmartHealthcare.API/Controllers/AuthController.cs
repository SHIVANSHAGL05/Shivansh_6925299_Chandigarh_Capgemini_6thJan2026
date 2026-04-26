using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHealthcare.Core.DTOs;
using SmartHealthcare.Core.Interfaces;

namespace SmartHealthcare.API.Controllers;

[Route("api/auth")]
public class AuthController : BaseApiController
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger      = logger;
    }

    /// <summary>Register a new user (Admin/Doctor/Patient)</summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _logger.LogInformation("Registration attempt for {Email}", dto.Email);
        var result = await _authService.RegisterAsync(dto, GetIpAddress());
        return HandleResponse(result);
    }

    /// <summary>Login and receive JWT + refresh token</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _logger.LogInformation("Login attempt for {Email}", dto.Email);
        var result = await _authService.LoginAsync(dto, GetIpAddress());

        if (result.Success)
            _logger.LogInformation("Login successful for {Email}", dto.Email);
        else
            _logger.LogWarning("Login failed for {Email}", dto.Email);

        return HandleResponse(result);
    }

    /// <summary>Refresh access token using refresh token</summary>
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto dto)
    {
        var result = await _authService.RefreshTokenAsync(dto.RefreshToken, GetIpAddress());
        return HandleResponse(result);
    }

    /// <summary>Revoke refresh token (logout)</summary>
    [HttpPost("revoke-token")]
    [Authorize]
    public async Task<IActionResult> RevokeToken([FromBody] RefreshTokenDto dto)
    {
        var result = await _authService.RevokeTokenAsync(dto.RefreshToken, GetIpAddress());
        return HandleResponse(result);
    }
}
