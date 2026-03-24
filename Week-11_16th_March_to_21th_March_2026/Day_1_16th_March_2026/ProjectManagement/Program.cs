using ProductManagement.Filters;

var builder = WebApplication.CreateBuilder(args);

// ─────────────────────────────────────────────────────────────────────────────
// 1. Register MVC with GLOBAL filters
//
//    Filters added here apply to EVERY controller action in the application
//    without needing any [Attribute] on individual controllers or actions.
//
//    Filter execution order (outermost → innermost):
//      Global filters → Controller filters → Action filters
//
//    We register both filters globally:
//      • CustomExceptionFilter — catches any unhandled exception app-wide
//      • LogActionFilter       — logs every action app-wide
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.AddControllersWithViews(options =>
{
    // Global exception filter — wraps all actions with friendly error handling.
    // Registered FIRST so it is outermost and catches exceptions from all filters.
    options.Filters.Add<CustomExceptionFilter>();

    // Global logging filter — logs action name + timestamp for every request.
    options.Filters.Add<LogActionFilter>();
});

// ─────────────────────────────────────────────────────────────────────────────
// 2. Register the filter classes with DI so constructor injection works.
//    (Required because our filters use ILogger<T> in their constructors.)
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.AddScoped<LogActionFilter>();
builder.Services.AddScoped<CustomExceptionFilter>();

var app = builder.Build();

// ─────────────────────────────────────────────────────────────────────────────
// 3. Middleware pipeline
// ─────────────────────────────────────────────────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Default route → ProductController.Index
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Product}/{action=Index}/{id?}");

app.Run();
