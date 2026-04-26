using LearningPlatform.API.Models;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.API.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        // Only seed if no users exist
        if (await db.Users.AnyAsync()) return;

        var admin = new User
        {
            Username = "admin",
            Email = "admin@learnhub.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            Role = "Admin"
        };

        var instructor = new User
        {
            Username = "demo_instructor",
            Email = "instructor@learnhub.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Instructor@123"),
            Role = "Instructor"
        };

        var student = new User
        {
            Username = "demo_student",
            Email = "student@learnhub.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Student@123"),
            Role = "Student"
        };

        db.Users.AddRange(admin, instructor, student);
        await db.SaveChangesAsync();

        // Add profiles
        db.Profiles.AddRange(
            new Profile { UserId = admin.Id, FullName = "System Admin" },
            new Profile { UserId = instructor.Id, FullName = "Demo Instructor", Bio = "Experienced .NET developer" },
            new Profile { UserId = student.Id, FullName = "Demo Student" }
        );
        await db.SaveChangesAsync();

        // Seed a sample course
        var course = new Course
        {
            Title = "Complete ASP.NET Core with .NET 10",
            Description = "Master ASP.NET Core with Razor Pages, Web API, Entity Framework Core, JWT Authentication, and more in this comprehensive bootcamp.",
            Category = "Programming",
            Price = 1299,
            Level = "Beginner",
            IsPublished = true,
            InstructorId = instructor.Id
        };
        db.Courses.Add(course);
        await db.SaveChangesAsync();

        db.Lessons.AddRange(
            new Lesson { Title = "Introduction to .NET 10", Content = "Overview of the .NET ecosystem and what's new in .NET 10.", Order = 1, DurationMinutes = 20, CourseId = course.Id },
            new Lesson { Title = "Setting Up Your Dev Environment", Content = "Install Visual Studio 2022, .NET 10 SDK, and SQL Server.", Order = 2, DurationMinutes = 15, CourseId = course.Id },
            new Lesson { Title = "Your First ASP.NET Core App", Content = "Build and run your first web application.", Order = 3, DurationMinutes = 30, CourseId = course.Id },
            new Lesson { Title = "Razor Pages Deep Dive", Content = "Learn the Razor Pages model in depth.", Order = 4, DurationMinutes = 45, CourseId = course.Id },
            new Lesson { Title = "Entity Framework Core & SQL Server", Content = "Code-first approach with EF Core migrations.", Order = 5, DurationMinutes = 60, CourseId = course.Id }
        );
        await db.SaveChangesAsync();
    }
}
