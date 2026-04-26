using Microsoft.EntityFrameworkCore;
using StudentRegistration.Data;

var builder = WebApplication.CreateBuilder(args);

// ─────────────────────────────────────────────────────────────────────────────
// 1. Register MVC services (controllers + Razor views)
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.AddControllersWithViews();

// ─────────────────────────────────────────────────────────────────────────────
// 2. Register ApplicationDbContext with the connection string from appsettings.json
//    - GetConnectionString("DefaultConnection") reads the value under
//      "ConnectionStrings" : { "DefaultConnection": "..." }
//    - UseSqlServer wires EF Core to SQL Server / LocalDB.
//      Swap for .UseSqlite(...) if you prefer SQLite.
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// ─────────────────────────────────────────────────────────────────────────────
// 3. TempData uses the session cookie provider by default in ASP.NET Core MVC.
//    No extra registration is needed unless you want distributed sessions.
// ─────────────────────────────────────────────────────────────────────────────

var app = builder.Build();

// ─────────────────────────────────────────────────────────────────────────────
// 4. Auto-apply pending EF Core migrations on startup (development convenience).
//    In production you would call `dotnet ef database update` in your CI/CD pipeline
//    and remove (or guard) this block.
// ─────────────────────────────────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate(); // Applies any pending migrations; creates DB if absent.
}

// ─────────────────────────────────────────────────────────────────────────────
// 5. Middleware pipeline
// ─────────────────────────────────────────────────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();   // Serves wwwroot (CSS, JS, images)
app.UseRouting();
app.UseAuthorization();

// Default route: /Controller/Action/Id
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Student}/{action=Register}/{id?}");

app.Run();
