using EmployeePortal.Data;
using EmployeePortal.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<CustomExceptionFilter>(); // outermost — catches everything
    options.Filters.Add<LogActionFilter>();        // logs every action
});

builder.Services.AddScoped<LogActionFilter>();
builder.Services.AddScoped<CustomExceptionFilter>();
builder.Services.AddScoped<SessionAuthFilter>();
builder.Services.AddSingleton<EmployeeRepository>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout        = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly    = true;   // JS cannot read the session cookie
    options.Cookie.IsEssential = true;   // works without cookie consent banner
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();        // ← MUST come before MapControllerRoute
app.UseAuthorization();

// Default route → AccountController.Login (admin must log in first)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
