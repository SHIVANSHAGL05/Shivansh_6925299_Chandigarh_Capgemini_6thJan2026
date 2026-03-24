using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// ── Services ──────────────────────────────────────────────────────────────
builder.Services.AddRazorPages();

// Task 7: Cookie authentication so User.IsInRole("Admin") works
// In production, replace with ASP.NET Core Identity + role management.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath  = "/Index";
        options.LogoutPath = "/Index";
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
    });

builder.Services.AddAuthorization();

// ── Pipeline ──────────────────────────────────────────────────────────────
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Authentication + Authorization middleware must come AFTER UseRouting
app.UseAuthentication();   // reads the cookie and populates User
app.UseAuthorization();    // enforces [Authorize] attributes

app.MapRazorPages();

app.Run();
