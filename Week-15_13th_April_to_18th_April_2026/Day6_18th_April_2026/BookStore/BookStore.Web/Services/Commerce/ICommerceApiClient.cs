namespace BookStore.Web.Services.Commerce;

public interface ICommerceApiClient
{
    Task<CommerceResult<IReadOnlyList<OrderApiDto>>> GetMyOrdersAsync(string accessToken, CancellationToken cancellationToken = default);
    Task<CommerceResult<IReadOnlyList<OrderApiDto>>> GetAllOrdersAsync(string accessToken, CancellationToken cancellationToken = default);
    Task<CommerceResult<int>> PlaceOrderAsync(PlaceOrderRequest request, string accessToken, CancellationToken cancellationToken = default);

    Task<CommerceResult<IReadOnlyList<WishlistApiDto>>> GetWishlistAsync(string accessToken, CancellationToken cancellationToken = default);
    Task<CommerceResult<string>> AddWishlistAsync(int bookId, string accessToken, CancellationToken cancellationToken = default);
    Task<CommerceResult<string>> RemoveWishlistAsync(int bookId, string accessToken, CancellationToken cancellationToken = default);

    Task<CommerceResult<IReadOnlyList<ReviewApiDto>>> GetReviewsAsync(int bookId, CancellationToken cancellationToken = default);
    Task<CommerceResult<int>> UpsertReviewAsync(int bookId, int rating, string comment, string accessToken, CancellationToken cancellationToken = default);

    Task<CommerceResult<ProfileApiDto>> GetProfileAsync(string accessToken, CancellationToken cancellationToken = default);
    Task<CommerceResult<string>> UpdateProfileAsync(UpdateProfileRequest request, string accessToken, CancellationToken cancellationToken = default);
    Task<CommerceResult<ReportSummaryApiDto>> GetReportSummaryAsync(string accessToken, CancellationToken cancellationToken = default);
    Task<CommerceResult<string>> UpdateOrderStatusAsync(int orderId, string status, string accessToken, CancellationToken cancellationToken = default);
    Task<CommerceResult<string>> CancelOrderAsync(int orderId, string? reason, string accessToken, CancellationToken cancellationToken = default);
    Task<CommerceResult<string>> ReturnOrderAsync(int orderId, string? reason, string accessToken, CancellationToken cancellationToken = default);
    Task<CommerceResult<string>> RefundOrderAsync(int orderId, string? reason, string accessToken, CancellationToken cancellationToken = default);
}
