using System.Security.Claims;
using LearningPlatform.API.Controllers;
using LearningPlatform.API.DTOs;
using LearningPlatform.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace LearningPlatform.Tests;

public class EnrollmentControllerTests : IDisposable
{
    private readonly LearningPlatform.API.Data.AppDbContext _db;
    private readonly EnrollmentController _controller;

    public EnrollmentControllerTests()
    {
        _db = TestHelpers.CreateInMemoryDb(Guid.NewGuid().ToString());
        var mapper = TestHelpers.CreateMapper();
        var logger = NullLogger<EnrollmentController>.Instance;
        _controller = new EnrollmentController(_db, mapper, logger);

        SeedDatabase();
        SetupUser(userId: 10, role: "Student");
    }

    private void SeedDatabase()
    {
        var instructor = new User { Id = 1, Username = "instr", Email = "i@i.com", PasswordHash = "h", Role = "Instructor" };
        var student = new User { Id = 10, Username = "student1", Email = "s@s.com", PasswordHash = "h", Role = "Student" };
        _db.Users.AddRange(instructor, student);

        _db.Courses.Add(new Course
        {
            Id = 1,
            Title = "Published Course",
            Description = "A published course",
            Category = "Programming",
            Price = 0,
            Level = "Beginner",
            IsPublished = true,
            InstructorId = 1,
            Instructor = instructor
        });
        _db.Courses.Add(new Course
        {
            Id = 2,
            Title = "Draft Course",
            Description = "Not published",
            Category = "Design",
            Price = 0,
            Level = "Beginner",
            IsPublished = false,
            InstructorId = 1,
            Instructor = instructor
        });

        _db.SaveChanges();
    }

    private void SetupUser(int userId, string role)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Name, "student1"),
            new(ClaimTypes.Role, role)
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }

    // ── Test 1: Enroll in published course succeeds ──────────
    [Fact]
    public async Task Enroll_PublishedCourse_ReturnsOk()
    {
        var dto = new EnrollDto { CourseId = 1 };
        var result = await _controller.Enroll(dto);

        Assert.IsType<OkObjectResult>(result);
    }

    // ── Test 2: Enroll in unpublished course returns BadRequest
    [Fact]
    public async Task Enroll_UnpublishedCourse_ReturnsBadRequest()
    {
        var dto = new EnrollDto { CourseId = 2 };
        var result = await _controller.Enroll(dto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    // ── Test 3: Double enrollment returns Conflict ───────────
    [Fact]
    public async Task Enroll_AlreadyEnrolled_ReturnsConflict()
    {
        // First enrollment
        await _controller.Enroll(new EnrollDto { CourseId = 1 });

        // Second attempt
        var result = await _controller.Enroll(new EnrollDto { CourseId = 1 });

        Assert.IsType<ConflictObjectResult>(result);
    }

    // ── Test 4: Enroll in non-existent course returns NotFound
    [Fact]
    public async Task Enroll_NonExistentCourse_ReturnsNotFound()
    {
        var dto = new EnrollDto { CourseId = 999 };
        var result = await _controller.Enroll(dto);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // ── Test 5: My enrollments returns correct list ──────────
    [Fact]
    public async Task MyEnrollments_ReturnsUserEnrollments()
    {
        await _controller.Enroll(new EnrollDto { CourseId = 1 });

        var result = await _controller.MyEnrollments();

        var ok = Assert.IsType<OkObjectResult>(result);
        var list = Assert.IsType<List<EnrollmentDto>>(ok.Value);
        Assert.Single(list);
        Assert.Equal(1, list[0].CourseId);
    }

    public void Dispose() => _db.Dispose();
}
