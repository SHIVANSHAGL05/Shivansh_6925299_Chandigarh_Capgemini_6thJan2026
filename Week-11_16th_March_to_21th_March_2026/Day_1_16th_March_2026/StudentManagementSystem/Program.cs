using StudentManagementSystem.Data;
using StudentManagementSystem.Filters;

var builder = WebApplication.CreateBuilder(args);

// ─────────────────────────────────────────────────────────────────────────────
// 1. MVC with GLOBAL filters
//    CustomExceptionFilter catches unhandled exceptions across all controllers.
//    LogActionFilter logs every action globally (StudentController also adds it
//    at class level for explicit demonstration).
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<CustomExceptionFilter>(); // global exception handling
    options.Filters.Add<LogActionFilter>();        // global action logging
});

// ─────────────────────────────────────────────────────────────────────────────
// 2. Register filters with DI (needed because they use constructor injection)
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.AddScoped<LogActionFilter>();
builder.Services.AddScoped<CustomExceptionFilter>();
builder.Services.AddScoped<SessionAuthFilter>();

// ─────────────────────────────────────────────────────────────────────────────
// 3. Register StudentRepository as a Singleton
//    Singleton = one instance for the entire app lifetime.
//    This ensures the in-memory student list is shared across all requests.
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.AddSingleton<StudentRepository>();

// ─────────────────────────────────────────────────────────────────────────────
// 4. Session setup
//    AddDistributedMemoryCache() provides the in-memory backing store.
//    AddSession() registers the session middleware service.
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout        = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly    = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// ─────────────────────────────────────────────────────────────────────────────
// 5. Middleware pipeline — ORDER MATTERS
// ─────────────────────────────────────────────────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();        // ← MUST be before MapControllerRoute

app.UseAuthorization();

// Default route → AccountController.Login (forces login first)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
