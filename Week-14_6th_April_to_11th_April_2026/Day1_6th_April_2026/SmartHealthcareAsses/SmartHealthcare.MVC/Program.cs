using Serilog;
using SmartHealthcare.MVC.Services;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/mvc-.log", rollingInterval: RollingInterval.Day)
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((ctx, cfg) =>
        cfg.ReadFrom.Configuration(ctx.Configuration)
           .WriteTo.Console()
           .WriteTo.File("Logs/mvc-.log", rollingInterval: RollingInterval.Day));

    // ── Session (stores JWT token) ─────────────────────────────────────────
    builder.Services.AddDistributedMemoryCache();
    builder.Services.AddSession(opt =>
    {
        opt.IdleTimeout        = TimeSpan.FromHours(2);
        opt.Cookie.HttpOnly    = true;
        opt.Cookie.IsEssential = true;
        opt.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

    // ── HttpClient for API calls ───────────────────────────────────────────
    var apiBase = builder.Configuration["ApiSettings:BaseUrl"]!;

    builder.Services.AddHttpClient("HealthcareAPI", client =>
    {
        client.BaseAddress = new Uri(apiBase);
        client.DefaultRequestHeaders.Add("Accept", "application/json");
    })
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        // Allow self-signed certs in development
        ServerCertificateCustomValidationCallback = (_, _, _, _) => true
    });

    // ── App Services ───────────────────────────────────────────────────────
    builder.Services.AddScoped<IApiService, ApiService>();
    builder.Services.AddHttpContextAccessor();

    // ── MVC ────────────────────────────────────────────────────────────────
    builder.Services.AddControllersWithViews();
    builder.Services.AddHttpsRedirection(opt => opt.HttpsPort = 7200);

    var app = builder.Build();

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();
    app.UseSession();
    app.UseSerilogRequestLogging();
    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "MVC host terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}
