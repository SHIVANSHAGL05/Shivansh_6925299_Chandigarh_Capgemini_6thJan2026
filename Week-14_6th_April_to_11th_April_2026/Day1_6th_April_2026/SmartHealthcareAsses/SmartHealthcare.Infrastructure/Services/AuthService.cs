using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SmartHealthcare.Core.Common;
using SmartHealthcare.Core.DTOs;
using SmartHealthcare.Core.Entities;
using SmartHealthcare.Core.Interfaces;
using SmartHealthcare.Infrastructure.Data;

namespace SmartHealthcare.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly AppDbContext _context;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration configuration,
        AppDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _context = context;
    }

    public async Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterDto dto, string ipAddress)
    {
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
            return ApiResponse<AuthResponseDto>.Fail("Email already registered.", 400);

        var user = new ApplicationUser
        {
            FirstName = dto.FirstName,
            LastName  = dto.LastName,
            Email     = dto.Email,
            UserName  = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            DateOfBirth = dto.DateOfBirth,
            Gender    = dto.Gender
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return ApiResponse<AuthResponseDto>.Fail("Registration failed.",
                errors: result.Errors.Select(e => e.Description).ToList());

        // Validate and assign role
        var validRoles = new[] { "Admin", "Doctor", "Patient" };
        var role = validRoles.Contains(dto.Role) ? dto.Role : "Patient";
        await _userManager.AddToRoleAsync(user, role);

        // Auto-create profile
        if (role == "Patient")
            _context.PatientProfiles.Add(new PatientProfile { UserId = user.Id });
        else if (role == "Doctor")
            _context.DoctorProfiles.Add(new DoctorProfile { UserId = user.Id, LicenseNumber = "PENDING" });

        await _context.SaveChangesAsync();

        return await BuildAuthResponseAsync(user, ipAddress);
    }

    public async Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto dto, string ipAddress)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null || !user.IsActive)
            return ApiResponse<AuthResponseDto>.Fail("Invalid credentials.", 401);

        var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: true);
        if (!result.Succeeded)
        {
            if (result.IsLockedOut)
                return ApiResponse<AuthResponseDto>.Fail("Account is locked out.", 401);
            return ApiResponse<AuthResponseDto>.Fail("Invalid credentials.", 401);
        }

        return await BuildAuthResponseAsync(user, ipAddress);
    }

    public async Task<ApiResponse<AuthResponseDto>> RefreshTokenAsync(string refreshToken, string ipAddress)
    {
        var token = await _context.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token == refreshToken);

        if (token == null || !token.IsActive)
            return ApiResponse<AuthResponseDto>.Fail("Invalid or expired refresh token.", 401);

        // Rotate token
        var newRefresh = GenerateRefreshToken(ipAddress);
        token.IsRevoked = true;
        token.RevokedByIp = ipAddress;
        token.ReplacedByToken = newRefresh.Token;

        token.User.RefreshTokens.Add(newRefresh);
        await _context.SaveChangesAsync();

        var roles = await _userManager.GetRolesAsync(token.User);
        var jwt   = GenerateJwtToken(token.User, roles);

        return ApiResponse<AuthResponseDto>.Ok(new AuthResponseDto
        {
            AccessToken   = jwt.token,
            RefreshToken  = newRefresh.Token,
            TokenExpiry   = jwt.expiry,
            Email         = token.User.Email!,
            FullName      = token.User.FullName,
            Roles         = roles,
            UserId        = token.User.Id
        });
    }

    public async Task<ApiResponse<bool>> RevokeTokenAsync(string refreshToken, string ipAddress)
    {
        var token = await _context.RefreshTokens.FirstOrDefaultAsync(r => r.Token == refreshToken);
        if (token == null || !token.IsActive)
            return ApiResponse<bool>.Fail("Token not found or already revoked.");

        token.IsRevoked = true;
        token.RevokedByIp = ipAddress;
        await _context.SaveChangesAsync();
        return ApiResponse<bool>.Ok(true, "Token revoked.");
    }

    // ── Helpers ────────────────────────────────────────────────────────────

    private async Task<ApiResponse<AuthResponseDto>> BuildAuthResponseAsync(ApplicationUser user, string ipAddress)
    {
        var roles      = await _userManager.GetRolesAsync(user);
        var jwt        = GenerateJwtToken(user, roles);
        var refresh    = GenerateRefreshToken(ipAddress);

        user.RefreshTokens.Add(refresh);
        await _context.SaveChangesAsync();

        return ApiResponse<AuthResponseDto>.Ok(new AuthResponseDto
        {
            AccessToken  = jwt.token,
            RefreshToken = refresh.Token,
            TokenExpiry  = jwt.expiry,
            Email        = user.Email!,
            FullName     = user.FullName,
            Roles        = roles,
            UserId       = user.Id
        });
    }

    private (string token, DateTime expiry) GenerateJwtToken(ApplicationUser user, IList<string> roles)
    {
        var key     = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds   = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiry  = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:ExpiryMinutes"] ?? "60"));

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name,           user.FullName),
            new(ClaimTypes.Email,          user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var token = new JwtSecurityToken(
            issuer:    _configuration["Jwt:Issuer"],
            audience:  _configuration["Jwt:Audience"],
            claims:    claims,
            expires:   expiry,
            signingCredentials: creds);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiry);
    }

    private static RefreshToken GenerateRefreshToken(string ipAddress) => new()
    {
        Token         = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
        ExpiresAt     = DateTime.UtcNow.AddDays(7),
        CreatedByIp   = ipAddress
    };
}
