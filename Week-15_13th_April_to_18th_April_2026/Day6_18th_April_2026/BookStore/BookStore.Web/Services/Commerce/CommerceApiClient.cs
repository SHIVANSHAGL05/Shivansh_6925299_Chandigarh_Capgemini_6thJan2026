using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace BookStore.Web.Services.Commerce;

public sealed class CommerceApiClient : ICommerceApiClient
{
    private readonly HttpClient _httpClient;

    public CommerceApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<CommerceResult<IReadOnlyList<OrderApiDto>>> GetMyOrdersAsync(string accessToken, CancellationToken cancellationToken = default)
        => SendAsync<IReadOnlyList<OrderApiDto>>(new HttpRequestMessage(HttpMethod.Get, "api/v1/orders/mine"), accessToken, cancellationToken);

    public Task<CommerceResult<IReadOnlyList<OrderApiDto>>> GetAllOrdersAsync(string accessToken, CancellationToken cancellationToken = default)
        => SendAsync<IReadOnlyList<OrderApiDto>>(new HttpRequestMessage(HttpMethod.Get, "api/v1/orders"), accessToken, cancellationToken);

    public Task<CommerceResult<int>> PlaceOrderAsync(PlaceOrderRequest request, string accessToken, CancellationToken cancellationToken = default)
    {
        var message = new HttpRequestMessage(HttpMethod.Post, "api/v1/orders")
        {
            Content = JsonContent.Create(request)
        };

        return SendAsync<int>(message, accessToken, cancellationToken);
    }

    public Task<CommerceResult<IReadOnlyList<WishlistApiDto>>> GetWishlistAsync(string accessToken, CancellationToken cancellationToken = default)
        => SendAsync<IReadOnlyList<WishlistApiDto>>(new HttpRequestMessage(HttpMethod.Get, "api/v1/wishlist/mine"), accessToken, cancellationToken);

    public Task<CommerceResult<string>> AddWishlistAsync(int bookId, string accessToken, CancellationToken cancellationToken = default)
        => SendAsync<string>(new HttpRequestMessage(HttpMethod.Post, $"api/v1/wishlist/{bookId}"), accessToken, cancellationToken);

    public Task<CommerceResult<string>> RemoveWishlistAsync(int bookId, string accessToken, CancellationToken cancellationToken = default)
        => SendAsync<string>(new HttpRequestMessage(HttpMethod.Delete, $"api/v1/wishlist/{bookId}"), accessToken, cancellationToken);

    public Task<CommerceResult<IReadOnlyList<ReviewApiDto>>> GetReviewsAsync(int bookId, CancellationToken cancellationToken = default)
        => SendAsync<IReadOnlyList<ReviewApiDto>>(new HttpRequestMessage(HttpMethod.Get, $"api/v1/books/{bookId}/reviews"), null, cancellationToken);

    public Task<CommerceResult<int>> UpsertReviewAsync(int bookId, int rating, string comment, string accessToken, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/v1/books/{bookId}/reviews")
        {
            Content = JsonContent.Create(new
            {
                rating,
                comment
            })
        };

        return SendAsync<int>(request, accessToken, cancellationToken);
    }

    public Task<CommerceResult<ProfileApiDto>> GetProfileAsync(string accessToken, CancellationToken cancellationToken = default)
        => SendAsync<ProfileApiDto>(new HttpRequestMessage(HttpMethod.Get, "api/v1/profile/me"), accessToken, cancellationToken);

    public Task<CommerceResult<string>> UpdateProfileAsync(UpdateProfileRequest request, string accessToken, CancellationToken cancellationToken = default)
    {
        var message = new HttpRequestMessage(HttpMethod.Put, "api/v1/profile/me")
        {
            Content = JsonContent.Create(request)
        };
        return SendAsync<string>(message, accessToken, cancellationToken);
    }

    public Task<CommerceResult<ReportSummaryApiDto>> GetReportSummaryAsync(string accessToken, CancellationToken cancellationToken = default)
        => SendAsync<ReportSummaryApiDto>(new HttpRequestMessage(HttpMethod.Get, "api/v1/reports/summary"), accessToken, cancellationToken);

    public Task<CommerceResult<string>> UpdateOrderStatusAsync(int orderId, string status, string accessToken, CancellationToken cancellationToken = default)
    {
        var message = new HttpRequestMessage(HttpMethod.Patch, $"api/v1/orders/{orderId}/status")
        {
            Content = JsonContent.Create(new { status })
        };

        return SendAsync<string>(message, accessToken, cancellationToken);
    }

    public Task<CommerceResult<string>> CancelOrderAsync(int orderId, string? reason, string accessToken, CancellationToken cancellationToken = default)
    {
        var message = new HttpRequestMessage(HttpMethod.Post, $"api/v1/orders/{orderId}/cancel")
        {
            Content = JsonContent.Create(new { reason = reason ?? string.Empty })
        };

        return SendAsync<string>(message, accessToken, cancellationToken);
    }

    public Task<CommerceResult<string>> ReturnOrderAsync(int orderId, string? reason, string accessToken, CancellationToken cancellationToken = default)
    {
        var message = new HttpRequestMessage(HttpMethod.Post, $"api/v1/orders/{orderId}/return")
        {
            Content = JsonContent.Create(new { reason = reason ?? string.Empty })
        };

        return SendAsync<string>(message, accessToken, cancellationToken);
    }

    public Task<CommerceResult<string>> RefundOrderAsync(int orderId, string? reason, string accessToken, CancellationToken cancellationToken = default)
    {
        var message = new HttpRequestMessage(HttpMethod.Post, $"api/v1/orders/{orderId}/refund")
        {
            Content = JsonContent.Create(new { reason = reason ?? string.Empty })
        };

        return SendAsync<string>(message, accessToken, cancellationToken);
    }

    private async Task<CommerceResult<T>> SendAsync<T>(HttpRequestMessage request, string? accessToken, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        try
        {
            using var response = await _httpClient.SendAsync(request, cancellationToken);
            var raw = await response.Content.ReadAsStringAsync(cancellationToken);
            CommerceEnvelope<JsonElement>? envelope = null;

            if (!string.IsNullOrWhiteSpace(raw))
            {
                envelope = JsonSerializer.Deserialize<CommerceEnvelope<JsonElement>>(raw, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return CommerceResult<T>.Fail("Session expired. Please login again.");
                }

                return CommerceResult<T>.Fail(envelope?.Message ?? $"Request failed ({(int)response.StatusCode}).");
            }

            if (envelope is null)
            {
                return CommerceResult<T>.Fail("Empty response from commerce API.");
            }

            if (!TryConvertData<T>(envelope.Data, out var data))
            {
                return CommerceResult<T>.Fail($"Commerce API returned an unexpected payload for {typeof(T).Name}.");
            }

            return CommerceResult<T>.Ok(data!, envelope.Message);
        }
        catch (Exception ex)
        {
            return CommerceResult<T>.Fail($"Commerce API unavailable: {ex.Message}");
        }
    }

    private static bool TryConvertData<T>(JsonElement dataElement, out T? data)
    {
        try
        {
            if (typeof(T) == typeof(string))
            {
                object? stringValue = dataElement.ValueKind switch
                {
                    JsonValueKind.String => dataElement.GetString(),
                    JsonValueKind.Number => dataElement.ToString(),
                    JsonValueKind.True => bool.TrueString,
                    JsonValueKind.False => bool.FalseString,
                    JsonValueKind.Null or JsonValueKind.Undefined => string.Empty,
                    _ => dataElement.GetRawText()
                };

                data = (T?)stringValue;
                return true;
            }

            if (typeof(T) == typeof(int))
            {
                if (dataElement.ValueKind == JsonValueKind.Number && dataElement.TryGetInt32(out var intValue))
                {
                    data = (T?)(object)intValue;
                    return true;
                }

                if (dataElement.ValueKind == JsonValueKind.String && int.TryParse(dataElement.GetString(), out intValue))
                {
                    data = (T?)(object)intValue;
                    return true;
                }
            }

            data = JsonSerializer.Deserialize<T>(dataElement.GetRawText(), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return true;
        }
        catch
        {
            data = default;
            return false;
        }
    }
}
