using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using SmartHealthcare.Core.Entities;
using SmartHealthcare.Core.Interfaces;
using SmartHealthcare.Infrastructure.Data;
using SmartHealthcare.Infrastructure.Services;
using SmartHealthcare.API.Middleware;

// ── Bootstrap Serilog ────────────────────────────────────────────────────────
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/api-.log", rollingInterval: RollingInterval.Day)
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Avoid native SNI DLL loading issues on Windows deep paths.
    AppContext.SetSwitch("Switch.Microsoft.Data.SqlClient.UseManagedNetworkingOnWindows", true);

    builder.Host.UseSerilog((ctx, cfg) =>
        cfg.ReadFrom.Configuration(ctx.Configuration)
           .WriteTo.Console()
           .WriteTo.File("Logs/api-.log", rollingInterval: RollingInterval.Day));

    // ── Database ─────────────────────────────────────────────────────────────
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
            sql => sql.MigrationsAssembly("SmartHealthcare.Infrastructure")));

    // ── Identity ──────────────────────────────────────────────────────────────
    builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>(opt =>
    {
        opt.Password.RequiredLength        = 8;
        opt.Password.RequireDigit          = true;
        opt.Password.RequireUppercase      = true;
        opt.Password.RequireNonAlphanumeric = false;
        opt.Lockout.MaxFailedAccessAttempts = 5;
        opt.Lockout.DefaultLockoutTimeSpan  = TimeSpan.FromMinutes(15);
        opt.User.RequireUniqueEmail         = true;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

    // ── JWT ───────────────────────────────────────────────────────────────────
    var jwtKey = builder.Configuration["Jwt:Key"]!;
    builder.Services.AddAuthentication(opt =>
    {
        opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        opt.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = builder.Configuration["Jwt:Issuer"],
            ValidAudience            = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew                = TimeSpan.Zero
        };
    });

    // ── Authorization Policies ────────────────────────────────────────────────
    builder.Services.AddAuthorization(opt =>
    {
        opt.AddPolicy("AdminOnly",   p => p.RequireRole("Admin"));
        opt.AddPolicy("DoctorOnly",  p => p.RequireRole("Doctor"));
        opt.AddPolicy("PatientOnly", p => p.RequireRole("Patient"));
        opt.AddPolicy("DoctorOrAdmin", p => p.RequireRole("Doctor", "Admin"));
    });

    // ── In-Memory Caching ─────────────────────────────────────────────────────
    builder.Services.AddMemoryCache();

    // ── AutoMapper ────────────────────────────────────────────────────────────
    builder.Services.AddAutoMapper(typeof(MappingProfile));

    // ── Application Services ──────────────────────────────────────────────────
    builder.Services.AddScoped<IAuthService,           AuthService>();
    builder.Services.AddScoped<IPatientService,        PatientService>();
    builder.Services.AddScoped<IDoctorService,         DoctorService>();
    builder.Services.AddScoped<IAppointmentService,    AppointmentService>();
    builder.Services.AddScoped<IPrescriptionService,   PrescriptionService>();
    builder.Services.AddScoped<IMedicineService,       MedicineService>();
    builder.Services.AddScoped<ISpecializationService, SpecializationService>();
    builder.Services.AddScoped<IDepartmentService,     DepartmentService>();
    builder.Services.AddScoped<IBillService,           BillService>();

    // ── Controllers + JSON Patch ──────────────────────────────────────────────
    builder.Services.AddControllers()
        .AddNewtonsoftJson(opt =>
            opt.SerializerSettings.ReferenceLoopHandling =
                Newtonsoft.Json.ReferenceLoopHandling.Ignore);

    // ── CORS ──────────────────────────────────────────────────────────────────
    var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()!;
    builder.Services.AddCors(opt => opt.AddPolicy("AllowMvc", policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()));

    // ── Swagger ───────────────────────────────────────────────────────────────
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title       = "Smart Healthcare Management API",
            Version     = "v1",
            Description = "REST API for Smart Healthcare Management System",
            Contact     = new OpenApiContact { Name = "Healthcare Dev Team" }
        });

        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Type        = SecuritySchemeType.Http,
            Scheme      = "bearer",
            BearerFormat = "JWT",
            Description = "Enter JWT token (without 'Bearer ' prefix)"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                },
                Array.Empty<string>()
            }
        });
    });

    // ── HTTPS ─────────────────────────────────────────────────────────────────
    builder.Services.AddHttpsRedirection(opt => opt.HttpsPort = 7100);

    var app = builder.Build();

    // ── Middleware Pipeline ───────────────────────────────────────────────────
    app.UseMiddleware<GlobalExceptionMiddleware>();
    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Healthcare API v1"));
    }

    app.UseHttpsRedirection();
    app.UseCors("AllowMvc");
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    // ── Auto-migrate + seed admin ─────────────────────────────────────────────
    using (var scope = app.Services.CreateScope())
    {
        var db          = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userMgr     = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleMgr     = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

        await db.Database.MigrateAsync();
        await DbSeeder.SeedAsync(userMgr, roleMgr, db);
    }

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "API host terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}
