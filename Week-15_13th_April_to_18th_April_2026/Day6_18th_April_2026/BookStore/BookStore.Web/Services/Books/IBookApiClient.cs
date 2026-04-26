using BookStore.Web.Models.ViewModels;

namespace BookStore.Web.Services.Books;

public interface IBookApiClient
{
    Task<BookApiResult<IReadOnlyList<InventoryItemViewModel>>> GetInventoryAsync(string? accessToken, CancellationToken cancellationToken = default);
    Task<BookApiResult<int>> CreateBookAsync(CreateBookViewModel model, string accessToken, CancellationToken cancellationToken = default);
    Task<BookApiResult<int>> UpdateBookAsync(int bookId, CreateBookViewModel model, string accessToken, CancellationToken cancellationToken = default);
    Task<BookApiResult<string>> DeleteBookAsync(int bookId, string accessToken, CancellationToken cancellationToken = default);
    Task<BookApiResult<string>> UploadBookImageAsync(IFormFile imageFile, string accessToken, CancellationToken cancellationToken = default);
}
