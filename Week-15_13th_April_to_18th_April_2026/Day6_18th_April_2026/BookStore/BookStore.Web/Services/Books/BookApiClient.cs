using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using BookStore.Web.Models.ViewModels;

namespace BookStore.Web.Services.Books;

public sealed class BookApiClient : IBookApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private sealed class MessageEnvelope
    {
        public string? Message { get; init; }
    }

    private readonly HttpClient _httpClient;

    public BookApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<BookApiResult<IReadOnlyList<InventoryItemViewModel>>> GetInventoryAsync(string? accessToken, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "api/v1/books");

        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        try
        {
            using var response = await _httpClient.SendAsync(request, cancellationToken);
            var raw = await response.Content.ReadAsStringAsync(cancellationToken);
            BookApiEnvelope<List<BookApiDto>>? envelope = null;
            if (!string.IsNullOrWhiteSpace(raw))
            {
                envelope = JsonSerializer.Deserialize<BookApiEnvelope<List<BookApiDto>>>(raw, JsonOptions);
            }

            if (!response.IsSuccessStatusCode)
            {
                var message = envelope?.Message;
                if (string.IsNullOrWhiteSpace(message))
                {
                    message = $"Books API request failed ({(int)response.StatusCode}).";
                }

                return BookApiResult<IReadOnlyList<InventoryItemViewModel>>.Fail(message);
            }

            var items = (envelope?.Data ?? [])
                .Select(x => new InventoryItemViewModel
                {
                    BookId = x.BookId,
                    Title = x.Title,
                    Author = x.Author,
                    Isbn = x.Isbn,
                    Category = x.Category,
                    Publisher = x.Publisher,
                    Price = x.Price,
                    Stock = x.Stock,
                    ImageUrl = x.ImageUrl,
                    AverageRating = x.AverageRating,
                    ReviewCount = x.ReviewCount
                })
                .ToList();

            return BookApiResult<IReadOnlyList<InventoryItemViewModel>>.Ok(items);
        }
        catch (Exception ex)
        {
            var baseUrl = _httpClient.BaseAddress?.ToString() ?? "(no base URL configured)";
            return BookApiResult<IReadOnlyList<InventoryItemViewModel>>.Fail($"Unable to connect to books API at {baseUrl}. {ex.Message}");
        }
    }

    public async Task<BookApiResult<int>> CreateBookAsync(CreateBookViewModel model, string accessToken, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "api/v1/books");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Content = JsonContent.Create(new
        {
            model.Title,
            ISBN = model.Isbn,
            model.Author,
            model.Category,
            model.Publisher,
            model.ImageUrl,
            model.Price,
            model.Stock
        });

        try
        {
            using var response = await _httpClient.SendAsync(request, cancellationToken);
            var raw = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var message = ExtractApiMessage(raw);
                if (string.IsNullOrWhiteSpace(message))
                {
                    message = response.StatusCode switch
                    {
                        System.Net.HttpStatusCode.Unauthorized => "Session expired. Please login again.",
                        System.Net.HttpStatusCode.Forbidden => "You do not have permission to add books.",
                        _ => $"Book creation failed ({(int)response.StatusCode})."
                    };
                }

                return BookApiResult<int>.Fail(message);
            }

            BookApiEnvelope<int>? envelope = null;
            if (!string.IsNullOrWhiteSpace(raw))
            {
                envelope = JsonSerializer.Deserialize<BookApiEnvelope<int>>(raw, JsonOptions);
            }

            return BookApiResult<int>.Ok(envelope?.Data ?? 0, envelope?.Message ?? "Book created.");
        }
        catch (Exception ex)
        {
            var baseUrl = _httpClient.BaseAddress?.ToString() ?? "(no base URL configured)";
            return BookApiResult<int>.Fail($"Unable to connect to books API at {baseUrl}. {ex.Message}");
        }
    }

    public async Task<BookApiResult<int>> UpdateBookAsync(int bookId, CreateBookViewModel model, string accessToken, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Put, $"api/v1/books/{bookId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Content = JsonContent.Create(new
        {
            model.Title,
            ISBN = model.Isbn,
            model.Author,
            model.Category,
            model.Publisher,
            model.ImageUrl,
            model.Price,
            model.Stock
        });

        try
        {
            using var response = await _httpClient.SendAsync(request, cancellationToken);
            var raw = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var message = ExtractApiMessage(raw);
                if (string.IsNullOrWhiteSpace(message))
                {
                    message = response.StatusCode switch
                    {
                        System.Net.HttpStatusCode.Unauthorized => "Session expired. Please login again.",
                        System.Net.HttpStatusCode.Forbidden => "You do not have permission to update books.",
                        _ => $"Book update failed ({(int)response.StatusCode})."
                    };
                }

                return BookApiResult<int>.Fail(message);
            }

            BookApiEnvelope<int>? envelope = null;
            if (!string.IsNullOrWhiteSpace(raw))
            {
                envelope = JsonSerializer.Deserialize<BookApiEnvelope<int>>(raw, JsonOptions);
            }

            return BookApiResult<int>.Ok(envelope?.Data ?? bookId, envelope?.Message ?? "Book updated.");
        }
        catch (Exception ex)
        {
            var baseUrl = _httpClient.BaseAddress?.ToString() ?? "(no base URL configured)";
            return BookApiResult<int>.Fail($"Unable to connect to books API at {baseUrl}. {ex.Message}");
        }
    }

    private static string? ExtractApiMessage(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return null;
        }

        try
        {
            var envelope = JsonSerializer.Deserialize<MessageEnvelope>(raw, JsonOptions);
            return envelope?.Message;
        }
        catch
        {
            return null;
        }
    }

    public async Task<BookApiResult<string>> DeleteBookAsync(int bookId, string accessToken, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, $"api/v1/books/{bookId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        try
        {
            using var response = await _httpClient.SendAsync(request, cancellationToken);
            var raw = await response.Content.ReadAsStringAsync(cancellationToken);
            BookApiEnvelope<string>? envelope = null;
            if (!string.IsNullOrWhiteSpace(raw))
            {
                envelope = JsonSerializer.Deserialize<BookApiEnvelope<string>>(raw, JsonOptions);
            }

            if (!response.IsSuccessStatusCode)
            {
                var message = envelope?.Message;
                if (string.IsNullOrWhiteSpace(message))
                {
                    message = response.StatusCode switch
                    {
                        System.Net.HttpStatusCode.Unauthorized => "Session expired. Please login again.",
                        System.Net.HttpStatusCode.Forbidden => "You do not have permission to delete books.",
                        _ => $"Book delete failed ({(int)response.StatusCode})."
                    };
                }

                return BookApiResult<string>.Fail(message);
            }

            return BookApiResult<string>.Ok(envelope?.Data ?? "ok", envelope?.Message ?? "Book deleted.");
        }
        catch (Exception ex)
        {
            var baseUrl = _httpClient.BaseAddress?.ToString() ?? "(no base URL configured)";
            return BookApiResult<string>.Fail($"Unable to connect to books API at {baseUrl}. {ex.Message}");
        }
    }

    public async Task<BookApiResult<string>> UploadBookImageAsync(IFormFile imageFile, string accessToken, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "api/v1/books/upload-image");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var content = new MultipartFormDataContent();
        await using var stream = imageFile.OpenReadStream();
        using var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(imageFile.ContentType);
        content.Add(fileContent, "file", imageFile.FileName);

        request.Content = content;

        try
        {
            using var response = await _httpClient.SendAsync(request, cancellationToken);
            var raw = await response.Content.ReadAsStringAsync(cancellationToken);
            BookApiEnvelope<string>? envelope = null;
            if (!string.IsNullOrWhiteSpace(raw))
            {
                envelope = JsonSerializer.Deserialize<BookApiEnvelope<string>>(raw, JsonOptions);
            }

            if (!response.IsSuccessStatusCode)
            {
                var message = envelope?.Message;
                if (string.IsNullOrWhiteSpace(message))
                {
                    message = response.StatusCode switch
                    {
                        System.Net.HttpStatusCode.Unauthorized => "Session expired. Please login again.",
                        System.Net.HttpStatusCode.Forbidden => "You do not have permission to upload images.",
                        _ => $"Image upload failed ({(int)response.StatusCode})."
                    };
                }

                return BookApiResult<string>.Fail(message);
            }

            if (string.IsNullOrWhiteSpace(envelope?.Data))
            {
                return BookApiResult<string>.Fail("Image uploaded, but URL was not returned by API.");
            }

            return BookApiResult<string>.Ok(envelope.Data, envelope.Message);
        }
        catch (Exception ex)
        {
            var baseUrl = _httpClient.BaseAddress?.ToString() ?? "(no base URL configured)";
            return BookApiResult<string>.Fail($"Unable to connect to books API at {baseUrl}. {ex.Message}");
        }
    }
}
