using LearningPlatform.API.Controllers;
using LearningPlatform.API.Data;
using LearningPlatform.API.DTOs;
using LearningPlatform.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace LearningPlatform.Tests;

public class CoursesControllerTests : IDisposable
{
    private readonly AppDbContext _db;
    private readonly CoursesController _controller;

    public CoursesControllerTests()
    {
        _db = TestHelpers.CreateInMemoryDb(Guid.NewGuid().ToString());
        var mapper = TestHelpers.CreateMapper();
        var cache = new MemoryCache(new MemoryCacheOptions());
        var logger = NullLogger<CoursesController>.Instance;
        _controller = new CoursesController(_db, mapper, cache, logger);

        SeedDatabase();
    }

    private void SeedDatabase()
    {
        var instructor = new User
        {
            Id = 1,
            Username = "instructor1",
            Email = "instructor@test.com",
            PasswordHash = "hash",
            Role = "Instructor"
        };
        _db.Users.Add(instructor);

        var course = new Course
        {
            Id = 1,
            Title = "ASP.NET Core Masterclass",
            Description = "Learn ASP.NET Core from scratch to advanced",
            Category = "Programming",
            Price = 999,
            Level = "Beginner",
            IsPublished = true,
            InstructorId = 1,
            Instructor = instructor
        };
        _db.Courses.Add(course);

        var lesson = new Lesson
        {
            Id = 1,
            Title = "Introduction to .NET",
            Content = "Welcome to .NET 10",
            Order = 1,
            DurationMinutes = 15,
            CourseId = 1
        };
        _db.Lessons.Add(lesson);

        _db.SaveChanges();
    }

    // ── Test 1: Get Course By Id (found) ────────────────────
    [Fact]
    public async Task GetById_ExistingCourse_ReturnsOkWithCourseDto()
    {
        // Act
        var result = await _controller.GetById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<CourseDto>(okResult.Value);
        Assert.Equal(1, dto.Id);
        Assert.Equal("ASP.NET Core Masterclass", dto.Title);
        Assert.Equal("instructor1", dto.InstructorName);
        Assert.Equal(1, dto.LessonCount);
    }

    // ── Test 2: Get Course By Id (not found) ────────────────
    [Fact]
    public async Task GetById_NonExistentCourse_ReturnsNotFound()
    {
        // Act
        var result = await _controller.GetById(999);

        // Assert
        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.NotNull(notFound.Value);
    }

    // ── Test 3: Get All Courses returns paged result ─────────
    [Fact]
    public async Task GetAll_ReturnsPublishedCourses()
    {
        // Act
        var result = await _controller.GetAll(1, 10, null);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var paged = Assert.IsType<PagedResult<CourseDto>>(okResult.Value);
        Assert.Equal(1, paged.TotalCount);
        Assert.Single(paged.Items);
    }

    // ── Test 4: Search filters courses correctly ─────────────
    [Fact]
    public async Task GetAll_WithSearch_FiltersCorrectly()
    {
        // Act
        var resultMatch = await _controller.GetAll(1, 10, "ASP.NET");
        var resultNoMatch = await _controller.GetAll(1, 10, "Python");

        // Assert
        var okMatch = Assert.IsType<OkObjectResult>(resultMatch);
        var pagedMatch = Assert.IsType<PagedResult<CourseDto>>(okMatch.Value);
        Assert.Equal(1, pagedMatch.TotalCount);

        var okNoMatch = Assert.IsType<OkObjectResult>(resultNoMatch);
        var pagedNoMatch = Assert.IsType<PagedResult<CourseDto>>(okNoMatch.Value);
        Assert.Equal(0, pagedNoMatch.TotalCount);
    }

    // ── Test 5: Get by Category returns correct courses ──────
    [Fact]
    public async Task GetByCategory_ExistingCategory_ReturnsCourses()
    {
        // Act
        var result = await _controller.GetByCategory("Programming");

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var courses = Assert.IsType<List<CourseDto>>(ok.Value);
        Assert.Single(courses);
        Assert.Equal("Programming", courses[0].Category);
    }

    // ── Test 6: Get by Category (no match) ──────────────────
    [Fact]
    public async Task GetByCategory_UnknownCategory_ReturnsEmptyList()
    {
        var result = await _controller.GetByCategory("Quantum Physics");

        var ok = Assert.IsType<OkObjectResult>(result);
        var courses = Assert.IsType<List<CourseDto>>(ok.Value);
        Assert.Empty(courses);
    }

    // ── Test 7: Create Course — Invalid model (missing title) 
    [Fact]
    public async Task Create_InvalidModel_ReturnsBadRequest()
    {
        // Simulate model validation failure
        _controller.ModelState.AddModelError("Title", "Title is required");

        var dto = new CreateCourseDto
        {
            Title = "",           // invalid
            Description = "Some description that is long enough",
            Category = "Programming",
            Price = 0,
            Level = "Beginner"
        };

        // Act
        var result = await _controller.Create(dto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    // ── Test 8: Unpublished courses are hidden from list ─────
    [Fact]
    public async Task GetAll_UnpublishedCourse_NotReturned()
    {
        _db.Courses.Add(new Course
        {
            Id = 2,
            Title = "Draft Course",
            Description = "Not published yet",
            Category = "Design",
            Price = 0,
            Level = "Beginner",
            IsPublished = false,        // hidden
            InstructorId = 1
        });
        await _db.SaveChangesAsync();

        var result = await _controller.GetAll(1, 10, null);
        var ok = Assert.IsType<OkObjectResult>(result);
        var paged = Assert.IsType<PagedResult<CourseDto>>(ok.Value);

        // Only the published course from seed should appear
        Assert.Equal(1, paged.TotalCount);
    }

    public void Dispose() => _db.Dispose();
}
