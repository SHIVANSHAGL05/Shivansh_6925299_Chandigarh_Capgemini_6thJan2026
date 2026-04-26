using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SmartHealthcare.Core.Common;

namespace SmartHealthcare.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next   = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message) = ex switch
        {
            ArgumentNullException      => (HttpStatusCode.BadRequest,            "A required argument was null."),
            ArgumentException          => (HttpStatusCode.BadRequest,            ex.Message),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized,         "Unauthorized access."),
            KeyNotFoundException       => (HttpStatusCode.NotFound,              ex.Message),
            DbUpdateConcurrencyException => (HttpStatusCode.Conflict,            "A concurrency conflict occurred."),
            DbUpdateException          => (HttpStatusCode.InternalServerError,   "A database error occurred."),
            OperationCanceledException => (HttpStatusCode.ServiceUnavailable,    "The operation was cancelled."),
            _                          => (HttpStatusCode.InternalServerError,   "An unexpected error occurred.")
        };

        context.Response.StatusCode = (int)statusCode;

        var response = new ErrorResponse
        {
            Message    = message,
            StatusCode = (int)statusCode,
            Timestamp  = DateTime.UtcNow,
            Details    = context.RequestServices
                             .GetRequiredService<IWebHostEnvironment>()
                             .IsDevelopment()
                         ? ex.ToString() : null
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
