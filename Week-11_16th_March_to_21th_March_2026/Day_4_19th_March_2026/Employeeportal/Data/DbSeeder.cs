using Microsoft.AspNetCore.Identity;

namespace EmployeePortal.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            // Seed Roles
            string[] roles = { "Admin", "Employee" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Seed Admin User
            var adminEmail = "admin@portal.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(adminUser, "Admin@1234");
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // Seed Employee User
            var empEmail = "employee@portal.com";
            var empUser = await userManager.FindByEmailAsync(empEmail);
            if (empUser == null)
            {
                empUser = new IdentityUser
                {
                    UserName = empEmail,
                    Email = empEmail,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(empUser, "Emp@1234");
                await userManager.AddToRoleAsync(empUser, "Employee");
            }
        }
    }
}
