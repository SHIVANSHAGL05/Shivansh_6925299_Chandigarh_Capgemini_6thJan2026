using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CardService.Middleware;

public static class JwtMiddlewareExtensions
{
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration config)
    {
        var jwtSection = config.GetSection("Jwt");
        var key        = jwtSection["Key"] ?? throw new InvalidOperationException("JWT Key not configured.");
        var issuer     = jwtSection["Issuer"] ?? "BankingApp";
        var audience   = jwtSection["Audience"] ?? "BankingApp";

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer           = true,
                ValidateAudience         = true,
                ValidateLifetime         = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer              = issuer,
                ValidAudience            = audience,
                IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ClockSkew                = TimeSpan.Zero
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = ctx =>
                {
                    ctx.Response.Headers.Append("Token-Expired",
                        ctx.Exception is SecurityTokenExpiredException ? "true" : "false");
                    return Task.CompletedTask;
                }
            };
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly",        policy => policy.RequireRole("Admin"));
            options.AddPolicy("CustomerOrAdmin",  policy => policy.RequireRole("Customer", "Admin"));
        });

        return services;
    }
}
