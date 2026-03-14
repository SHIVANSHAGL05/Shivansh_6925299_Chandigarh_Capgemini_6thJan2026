using Microsoft.EntityFrameworkCore;
using StudentPortal.Data;
using StudentPortal.Middleware;
using StudentPortal.Services;

var builder = WebApplication.CreateBuilder(args);

// ── Service Registration ─────────────────────────────────────────
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Singleton: one shared instance across all requests and middleware
builder.Services.AddSingleton<IRequestLogService, RequestLogService>();

// ── Build App ───────────────────────────────────────────────────
var app = builder.Build();

// ── Middleware Pipeline (ORDER MATTERS) ─────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();       // Static files bypass request tracking (by design)
app.UseRouting();           // Route must be resolved before tracking middleware

// Custom middleware — registered after UseRouting so Path is fully resolved
app.UseMiddleware<RequestTrackingMiddleware>();

app.UseAuthorization();

// ── Conventional Routing ─────────────────────────────────────────
// Default route: /Students/Index → StudentsController.Index()
app.MapControllerRoute(
    name:    "default",
    pattern: "{controller=Students}/{action=Index}/{id?}");

app.Run();
