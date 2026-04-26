// Program.cs
using AuthPortal.Services;
using AuthPortal.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Session support — required for auth tracking
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Register services in DI container
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddSingleton<IStudentService, StudentService>();
builder.Services.AddSingleton<IOperationLogService, OperationLogService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Session must be before custom middleware
app.UseSession();

// Custom Auth Middleware — protects all /Admin/* routes
app.UseMiddleware<AuthenticationMiddleware>();

app.UseAuthorization();

// Conventional Routing
// /Students/*  → public
// /Admin/*     → protected by middleware
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Students}/{action=Index}/{id?}");

app.Run();