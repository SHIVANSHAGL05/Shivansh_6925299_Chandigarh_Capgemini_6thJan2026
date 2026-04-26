using BookStore.API.Data;
using BookStore.API.Dtos;
using BookStore.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.API.Controllers;

[ApiController]
[Route("api/v1/profile")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly BookStoreDbContext _dbContext;

    public ProfileController(BookStoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("me")]
    public async Task<IActionResult> Me(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized(ApiResponse<string>.Fail("Invalid user session."));
        }

        var user = await _dbContext.Users
            .Include(x => x.Profile)
            .FirstOrDefaultAsync(x => x.AppUserId == userId.Value, cancellationToken);

        if (user is null)
        {
            return NotFound(ApiResponse<string>.Fail("User not found."));
        }

        var data = new ProfileDto
        {
            FullName = user.FullName,
            Email = user.Email,
            Phone = user.Phone,
            Address = user.Profile?.Address ?? string.Empty,
            City = user.Profile?.City ?? string.Empty,
            Pincode = user.Profile?.Pincode ?? string.Empty
        };

        return Ok(ApiResponse<ProfileDto>.Ok(data));
    }

    [HttpPut("me")]
    public async Task<IActionResult> Update([FromBody] UpdateProfileDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var errors = string.Join(" ", ModelState.Values
                .SelectMany(x => x.Errors)
                .Select(x => x.ErrorMessage)
                .Where(x => !string.IsNullOrWhiteSpace(x)));
            return BadRequest(ApiResponse<string>.Fail(string.IsNullOrWhiteSpace(errors) ? "Invalid profile data." : errors));
        }

        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized(ApiResponse<string>.Fail("Invalid user session."));
        }

        var user = await _dbContext.Users
            .Include(x => x.Profile)
            .FirstOrDefaultAsync(x => x.AppUserId == userId.Value, cancellationToken);

        if (user is null)
        {
            return NotFound(ApiResponse<string>.Fail("User not found."));
        }

        user.FullName = dto.FullName;
        user.Phone = dto.Phone;

        if (user.Profile is null)
        {
            user.Profile = new UserProfile { UserId = user.AppUserId };
        }

        user.Profile.Address = dto.Address;
        user.Profile.City = dto.City;
        user.Profile.Pincode = dto.Pincode;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Ok(ApiResponse<string>.Ok("ok", "Profile updated."));
    }

    private int? GetUserId()
    {
        var raw = User.FindFirst("user_id")?.Value;
        return int.TryParse(raw, out var userId) ? userId : null;
    }
}
