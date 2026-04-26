using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RoleBasedAccessAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (request.Username == "shivansh" && request.Password == "admin123")
            {
                var token = GenerateJwtToken("user-001", request.Username, "Admin");
                return Ok(new { token, role = "Admin" });
            }

            if (request.Username == "shiva" && request.Password == "user123")
            {
                var token = GenerateJwtToken("user-002", request.Username, "User");
                return Ok(new { token, role = "User" });
            }

            return Unauthorized(new { message = "Invalid credentials." });
        }

        private string GenerateJwtToken(string userId, string username, string role)
        {
            var key = _configuration["Jwt:Key"] ?? "RoleBasedSecretKeyForJWT123!";
            var issuer = _configuration["Jwt:Issuer"] ?? "RoleBasedAccessAPI";
            var audience = _configuration["Jwt:Audience"] ?? "RoleBasedAccessAPIUsers";

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            };

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
