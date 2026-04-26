using EmployeePortal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EmployeePortal.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Apply migrations
            await context.Database.MigrateAsync();

            // Seed roles
            string[] roles = { "Admin", "Employee" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Seed Admin user
            var adminEmail = "admin@portal.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
                var result = await userManager.CreateAsync(admin, "Admin@123");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, "Admin");
            }

            // Seed Employee user
            var empEmail = "emp@portal.com";
            if (await userManager.FindByEmailAsync(empEmail) == null)
            {
                var emp = new IdentityUser { UserName = empEmail, Email = empEmail, EmailConfirmed = true };
                var result = await userManager.CreateAsync(emp, "Emp@123");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(emp, "Employee");
            }

            // Seed sample employees
            if (!context.Employees.Any())
            {
                context.Employees.AddRange(
                    new Employee { Name = "Alice Johnson", Department = "Engineering", Salary = 85000 },
                    new Employee { Name = "Bob Smith", Department = "Marketing", Salary = 65000 },
                    new Employee { Name = "Carol White", Department = "HR", Salary = 60000 },
                    new Employee { Name = "David Brown", Department = "Engineering", Salary = 90000 },
                    new Employee { Name = "Eve Davis", Department = "Finance", Salary = 75000 }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}
