using BookStore.Web.Models.ViewModels;

namespace BookStore.Web.Services.Auth;

public interface IAuthApiClient
{
    Task<AuthResult> LoginAsync(LoginViewModel model, CancellationToken cancellationToken = default);
    Task<AuthResult> RegisterAsync(RegisterViewModel model, CancellationToken cancellationToken = default);
}
