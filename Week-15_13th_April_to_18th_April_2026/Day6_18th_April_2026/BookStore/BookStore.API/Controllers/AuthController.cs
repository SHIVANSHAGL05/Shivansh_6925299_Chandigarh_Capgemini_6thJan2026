using BookStore.API.Data;
using BookStore.API.Dtos;
using BookStore.API.Models;
using BookStore.API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.API.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly BookStoreDbContext _dbContext;
    private readonly JwtTokenService _tokenService;
    private readonly IConfiguration _configuration;
    private readonly PasswordHasher<AppUser> _passwordHasher = new();

    public AuthController(BookStoreDbContext dbContext, JwtTokenService tokenService, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _tokenService = tokenService;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = string.Join(" ", ModelState.Values
                .SelectMany(x => x.Errors)
                .Select(x => x.ErrorMessage)
                .Where(x => !string.IsNullOrWhiteSpace(x)));
            return BadRequest(ApiResponse<string>.Fail(string.IsNullOrWhiteSpace(errors) ? "Invalid registration data." : errors));
        }

        var exists = await _dbContext.Users.AnyAsync(x => x.Email == dto.Email);
        if (exists)
        {
            return BadRequest(ApiResponse<string>.Fail("Email already registered."));
        }

        var selectedRole = string.Equals(dto.Role, "Admin", StringComparison.OrdinalIgnoreCase) ? "Admin" : "Customer";
        if (selectedRole == "Admin")
        {
            var adminKey = _configuration["AuthSettings:AdminRegistrationKey"];
            if (string.IsNullOrWhiteSpace(adminKey) || !string.Equals(dto.AdminRegistrationKey, adminKey, StringComparison.Ordinal))
            {
                return Unauthorized(ApiResponse<string>.Fail("Invalid admin registration key."));
            }
        }

        var role = await _dbContext.Roles.FirstOrDefaultAsync(x => x.RoleName == selectedRole);
        if (role is null)
        {
            return BadRequest(ApiResponse<string>.Fail("Role configuration is invalid."));
        }

        var user = new AppUser
        {
            FullName = dto.FullName,
            Email = dto.Email,
            Phone = dto.Phone,
            Role = selectedRole,
            RoleId = role.RoleId
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        _dbContext.UserProfiles.Add(new UserProfile
        {
            UserId = user.AppUserId,
            Address = dto.Address,
            City = dto.City,
            Pincode = dto.Pincode
        });
        await _dbContext.SaveChangesAsync();

        var token = _tokenService.GenerateToken(user);
        var response = new AuthResponseDto
        {
            Token = token,
            RefreshToken = Guid.NewGuid().ToString("N"),
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role
        };

        return Ok(ApiResponse<AuthResponseDto>.Ok(response, "Registration successful"));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<string>.Fail("Email and password are required."));
        }

        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == dto.Email);
        if (user is null)
        {
            return Unauthorized(ApiResponse<string>.Fail("Invalid email or password."));
        }

        var verified = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if (verified == PasswordVerificationResult.Failed)
        {
            return Unauthorized(ApiResponse<string>.Fail("Invalid email or password."));
        }

        var token = _tokenService.GenerateToken(user);
        var response = new AuthResponseDto
        {
            Token = token,
            RefreshToken = Guid.NewGuid().ToString("N"),
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role
        };

        return Ok(ApiResponse<AuthResponseDto>.Ok(response, "Login successful"));
    }
}
