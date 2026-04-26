using System.Text;
using BookStore.API.Data;
using BookStore.API.Middleware;
using BookStore.API.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddDbContext<BookStoreDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("AzureSqlConnection")
        ?? throw new InvalidOperationException("Azure SQL connection string is not configured.");

    options.UseSqlServer(connectionString);
});

builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddScoped<EmailNotificationService>();
builder.Services.AddScoped<BlobStorageService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("MvcClient", policy =>
        policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
});

var jwtSecret = builder.Configuration["JwtSettings:SecretKey"] ?? throw new InvalidOperationException("JWT secret not configured");
var jwtIssuer = builder.Configuration["JwtSettings:Issuer"] ?? "BookStore.API";
var jwtAudience = builder.Configuration["JwtSettings:Audience"] ?? "BookStore.Web";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.MapOpenApi();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BookStoreDbContext>();
    db.Database.Migrate();
}

app.UseRouting();

app.UseCors("MvcClient");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
