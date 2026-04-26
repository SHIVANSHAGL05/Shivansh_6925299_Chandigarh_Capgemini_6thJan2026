using System.Collections.Concurrent;
using Microsoft.AspNetCore.Identity;
using BookStore.Web.Models.ViewModels;

namespace BookStore.Web.Services.Auth;

public sealed class LocalAuthService : ILocalAuthService
{
    private readonly IConfiguration _configuration;
    private readonly PasswordHasher<LocalUser> _passwordHasher = new();
    private readonly ConcurrentDictionary<string, LocalUser> _users = new(StringComparer.OrdinalIgnoreCase);

    public LocalAuthService(IConfiguration configuration)
    {
        _configuration = configuration;

        // Seed a demo customer account for quick testing.
        SeedUser("Demo User", "user@bookstore.local", "+919999999999", "Customer@123!", "Customer");
        SeedUser("Demo Admin", "admin@bookstore.local", "+918888888888", "Admin@123!", "Admin");
    }

    public Task<LocalAuthResult> LoginAsync(LoginViewModel model, CancellationToken cancellationToken = default)
    {
        if (!_users.TryGetValue(model.Email, out var user))
        {
            return Task.FromResult(LocalAuthResult.Fail("Invalid email or password."));
        }

        var verify = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);
        if (verify == PasswordVerificationResult.Failed)
        {
            return Task.FromResult(LocalAuthResult.Fail("Invalid email or password."));
        }

        return Task.FromResult(LocalAuthResult.Ok(new LocalSessionUser
        {
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role
        }));
    }

    public Task<LocalAuthResult> RegisterAsync(RegisterViewModel model, CancellationToken cancellationToken = default)
    {
        if (_users.ContainsKey(model.Email))
        {
            return Task.FromResult(LocalAuthResult.Fail("Email already registered."));
        }

        var selectedRole = string.Equals(model.Role, "Admin", StringComparison.OrdinalIgnoreCase)
            ? "Admin"
            : "Customer";

        if (selectedRole == "Admin")
        {
            var configuredKey = _configuration["AuthSettings:AdminRegistrationKey"];
            if (string.IsNullOrWhiteSpace(configuredKey) || !string.Equals(model.AdminRegistrationKey, configuredKey, StringComparison.Ordinal))
            {
                return Task.FromResult(LocalAuthResult.Fail("Invalid admin registration key."));
            }
        }

        var user = new LocalUser
        {
            FullName = model.FullName,
            Email = model.Email,
            Phone = model.Phone,
            Role = selectedRole
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);
        _users[user.Email] = user;

        return Task.FromResult(LocalAuthResult.Ok(new LocalSessionUser
        {
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role
        }));
    }

    private void SeedUser(string fullName, string email, string phone, string password, string role)
    {
        var user = new LocalUser
        {
            FullName = fullName,
            Email = email,
            Phone = phone,
            Role = role
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, password);
        _users[user.Email] = user;
    }

    private sealed class LocalUser
    {
        public string FullName { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string Phone { get; init; } = string.Empty;
        public string Role { get; init; } = "Customer";
        public string PasswordHash { get; set; } = string.Empty;
    }
}
