using BookStore.Web.Models.ViewModels;

namespace BookStore.Web.Services.Auth;

public interface ILocalAuthService
{
    Task<LocalAuthResult> LoginAsync(LoginViewModel model, CancellationToken cancellationToken = default);
    Task<LocalAuthResult> RegisterAsync(RegisterViewModel model, CancellationToken cancellationToken = default);
}
