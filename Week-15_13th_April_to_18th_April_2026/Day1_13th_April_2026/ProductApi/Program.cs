using Azure.Identity;
using ProductApi.Data;
using ProductApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ✅ SAFE KEY VAULT HANDLING (NO CRASH)
var keyVaultUri = builder.Configuration["KeyVaultUri"];

if (builder.Environment.IsProduction())
{
    try
    {
        if (!string.IsNullOrWhiteSpace(keyVaultUri))
        {
            // builder.Configuration.AddAzureKeyVault(new Uri(keyVaultUri), new DefaultAzureCredential());
        }

        var requiredKeys = new[]
        {
            "ConnectionStrings:DefaultConnection",
            "Jwt:Key",
            "Jwt:Issuer",
            "Jwt:Audience",
            "BlobConnection",
            "BlobContainerName"
        };

        foreach (var key in requiredKeys)
        {
            if (string.IsNullOrWhiteSpace(builder.Configuration[key]))
            {
                Console.WriteLine($"WARNING: Missing configuration value: {key}");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Key Vault skipped: " + ex.Message);
    }
}

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DB
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<BlobService>();
builder.Services.AddScoped<TokenService>();

// JWT
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        var jwtKey = builder.Configuration["Jwt:Key"]!;
        var jwtIssuer = builder.Configuration["Jwt:Issuer"];
        var jwtAudience = builder.Configuration["Jwt:Audience"];

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = !string.IsNullOrWhiteSpace(jwtIssuer),
            ValidateAudience = !string.IsNullOrWhiteSpace(jwtAudience),
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Middleware
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();