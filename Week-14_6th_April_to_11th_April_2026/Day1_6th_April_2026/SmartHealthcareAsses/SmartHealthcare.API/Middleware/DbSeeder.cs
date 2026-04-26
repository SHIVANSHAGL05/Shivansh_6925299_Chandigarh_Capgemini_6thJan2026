using Microsoft.AspNetCore.Identity;
using SmartHealthcare.Core.Entities;
using SmartHealthcare.Infrastructure.Data;

namespace SmartHealthcare.API.Middleware;

public static class DbSeeder
{
    public static async Task SeedAsync(
        UserManager<ApplicationUser> userMgr,
        RoleManager<IdentityRole<int>> roleMgr,
        AppDbContext db)
    {
        // Ensure roles exist
        foreach (var role in new[] { "Admin", "Doctor", "Patient" })
        {
            if (!await roleMgr.RoleExistsAsync(role))
                await roleMgr.CreateAsync(new IdentityRole<int>(role));
        }

        // Seed admin
        const string adminEmail = "admin@healthcare.com";
        if (await userMgr.FindByEmailAsync(adminEmail) == null)
        {
            var admin = new ApplicationUser
            {
                FirstName   = "System",
                LastName    = "Admin",
                Email       = adminEmail,
                UserName    = adminEmail,
                DateOfBirth = new DateTime(1990, 1, 1),
                IsActive    = true
            };
            var result = await userMgr.CreateAsync(admin, "Admin@12345");
            if (result.Succeeded)
                await userMgr.AddToRoleAsync(admin, "Admin");
        }

        // Seed specializations
        if (!db.Specializations.Any())
        {
            db.Specializations.AddRange(
                new Specialization { Name = "Cardiology",      Description = "Heart and cardiovascular system" },
                new Specialization { Name = "Neurology",       Description = "Brain and nervous system" },
                new Specialization { Name = "Orthopedics",     Description = "Bones, joints and muscles" },
                new Specialization { Name = "Pediatrics",      Description = "Children's health" },
                new Specialization { Name = "Dermatology",     Description = "Skin conditions" },
                new Specialization { Name = "General Medicine", Description = "General health and wellness" },
                new Specialization { Name = "Gynecology",      Description = "Women's health" },
                new Specialization { Name = "Ophthalmology",   Description = "Eye care" }
            );
            await db.SaveChangesAsync();
        }

        // Seed sample medicines
        if (!db.Medicines.Any())
        {
            db.Medicines.AddRange(
                new Medicine { Name = "Paracetamol",   GenericName = "Acetaminophen", Category = "Analgesic",    RequiresPrescription = false },
                new Medicine { Name = "Amoxicillin",   GenericName = "Amoxicillin",   Category = "Antibiotic",   RequiresPrescription = true  },
                new Medicine { Name = "Metformin",     GenericName = "Metformin HCl", Category = "Antidiabetic", RequiresPrescription = true  },
                new Medicine { Name = "Amlodipine",    GenericName = "Amlodipine",    Category = "Antihypertensive", RequiresPrescription = true },
                new Medicine { Name = "Omeprazole",    GenericName = "Omeprazole",    Category = "Antacid",      RequiresPrescription = false },
                new Medicine { Name = "Ibuprofen",     GenericName = "Ibuprofen",     Category = "NSAID",        RequiresPrescription = false },
                new Medicine { Name = "Atorvastatin",  GenericName = "Atorvastatin",  Category = "Statin",       RequiresPrescription = true  },
                new Medicine { Name = "Azithromycin",  GenericName = "Azithromycin",  Category = "Antibiotic",   RequiresPrescription = true  }
            );
            await db.SaveChangesAsync();
        }
    }
}
