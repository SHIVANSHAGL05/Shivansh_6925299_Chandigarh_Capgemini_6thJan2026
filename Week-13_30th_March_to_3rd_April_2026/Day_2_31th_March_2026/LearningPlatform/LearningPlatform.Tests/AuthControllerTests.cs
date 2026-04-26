using LearningPlatform.API.Controllers;
using LearningPlatform.API.DTOs;
using LearningPlatform.API.Models;
using LearningPlatform.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace LearningPlatform.Tests;

public class AuthControllerTests : IDisposable
{
    private readonly LearningPlatform.API.Data.AppDbContext _db;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _db = TestHelpers.CreateInMemoryDb(Guid.NewGuid().ToString());
        _tokenServiceMock = new Mock<ITokenService>();
        var logger = NullLogger<AuthController>.Instance;
        _controller = new AuthController(_db, _tokenServiceMock.Object, logger);

        _tokenServiceMock
            .Setup(t => t.GenerateAccessToken(It.IsAny<User>()))
            .Returns("mock.jwt.token");

        _tokenServiceMock
            .Setup(t => t.GenerateRefreshToken())
            .Returns("mock-refresh-token");
    }

    // ── Test 1: Register new user succeeds ───────────────────
    [Fact]
    public async Task Register_ValidDto_ReturnsOk()
    {
        var dto = new RegisterDto
        {
            Username = "newuser",
            Email = "newuser@test.com",
            Password = "password123",
            Role = "Student"
        };

        var result = await _controller.Register(dto);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(ok.Value);
    }

    // ── Test 2: Register duplicate email returns Conflict ────
    [Fact]
    public async Task Register_DuplicateEmail_ReturnsConflict()
    {
        // Seed a user
        _db.Users.Add(new User
        {
            Username = "existing",
            Email = "taken@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("pass"),
            Role = "Student"
        });
        await _db.SaveChangesAsync();

        var dto = new RegisterDto
        {
            Username = "newguy",
            Email = "taken@test.com",
            Password = "password123",
            Role = "Student"
        };

        var result = await _controller.Register(dto);

        Assert.IsType<ConflictObjectResult>(result);
    }

    // ── Test 3: Login with valid credentials returns token ───
    [Fact]
    public async Task Login_ValidCredentials_ReturnsTokenResponse()
    {
        _db.Users.Add(new User
        {
            Username = "loginuser",
            Email = "login@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
            Role = "Student"
        });
        await _db.SaveChangesAsync();

        var dto = new LoginDto { Email = "login@test.com", Password = "password123" };
        var result = await _controller.Login(dto);

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<AuthResponseDto>(ok.Value);
        Assert.Equal("mock.jwt.token", response.Token);
        Assert.Equal("loginuser", response.Username);
    }

    // ── Test 4: Login with wrong password returns Unauthorized
    [Fact]
    public async Task Login_WrongPassword_ReturnsUnauthorized()
    {
        _db.Users.Add(new User
        {
            Username = "user2",
            Email = "user2@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword"),
            Role = "Student"
        });
        await _db.SaveChangesAsync();

        var dto = new LoginDto { Email = "user2@test.com", Password = "wrongpassword" };
        var result = await _controller.Login(dto);

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    // ── Test 5: Login with non-existent email returns Unauthorized
    [Fact]
    public async Task Login_NonExistentEmail_ReturnsUnauthorized()
    {
        var dto = new LoginDto { Email = "ghost@test.com", Password = "password123" };
        var result = await _controller.Login(dto);

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    // ── Test 6: Register with invalid model returns BadRequest
    [Fact]
    public async Task Register_InvalidModel_ReturnsBadRequest()
    {
        _controller.ModelState.AddModelError("Email", "Invalid email format");

        var dto = new RegisterDto
        {
            Username = "test",
            Email = "not-an-email",
            Password = "pass123",
            Role = "Student"
        };

        var result = await _controller.Register(dto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    public void Dispose() => _db.Dispose();
}
