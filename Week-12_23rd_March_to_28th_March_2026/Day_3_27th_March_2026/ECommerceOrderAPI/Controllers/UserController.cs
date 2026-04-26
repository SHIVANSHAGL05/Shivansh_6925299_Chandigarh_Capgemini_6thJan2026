using ECommerceOrderAPI.Data;
using ECommerceOrderAPI.DTOs;
using log4net;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceOrderAPI.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(UserController));
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                log.Warn("Login attempt with missing email or password");
                return BadRequest(new { message = "Email and password are required." });
            }

            log.Info($"Login attempt: {request.Email}");

            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == request.Email);

                if (user == null)
                {
                    log.Warn($"Login failed - User not found: {request.Email}");
                    return Unauthorized(new { message = "Invalid email or password." });
                }

                if (user.Password != request.Password)
                {
                    log.Warn($"Invalid password for user: {request.Email}");
                    return Unauthorized(new { message = "Invalid email or password." });
                }

                log.Info($"Login successful for user: {request.Email}");
                return Ok(new { message = "Login successful.", userId = user.Id, name = user.Name });
            }
            catch (Exception ex)
            {
                log.Error($"Exception during login for {request.Email}", ex);
                return StatusCode(500, new { message = "An error occurred during login." });
            }
        }
    }
}
