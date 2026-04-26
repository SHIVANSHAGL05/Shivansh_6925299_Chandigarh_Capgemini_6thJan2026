using EfCoreCodeFirstApproachDemo01.Models;
using Microsoft.EntityFrameworkCore;

namespace EfCoreCodeFirstApproachDemo01
{
    public class Program
    {
        public static void Main(string[] args)
        {

            
            
            var builder = WebApplication.CreateBuilder(args);

          
            // Add services to the container.
            builder.Services.AddControllersWithViews();
            var cs1 = builder.Configuration.GetConnectionString("cs1");
            builder.Services.AddDbContext<EmployeeDBContext>(options => options.UseSqlServer(cs1));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
