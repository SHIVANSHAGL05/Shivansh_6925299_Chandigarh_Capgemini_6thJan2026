using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SmartHealthcare.Core.Common;

namespace SmartHealthcare.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class BaseApiController : ControllerBase
{
    protected int GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(claim, out var id) ? id : 0;
    }

    protected string GetCurrentUserEmail() =>
        User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;

    protected string GetIpAddress() =>
        Request.Headers.ContainsKey("X-Forwarded-For")
            ? Request.Headers["X-Forwarded-For"].ToString()
            : HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "unknown";

    protected IActionResult HandleResponse<T>(ApiResponse<T> response) =>
        response.StatusCode switch
        {
            200 => Ok(response),
            201 => StatusCode(201, response),
            400 => BadRequest(response),
            401 => Unauthorized(response),
            404 => NotFound(response),
            _   => StatusCode(response.StatusCode, response)
        };
}
