namespace BookStore.Web.Services.Commerce;

public sealed class CommerceEnvelope<T>
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public T? Data { get; init; }
}

public sealed class CommerceResult<T>
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public T? Data { get; init; }

    public static CommerceResult<T> Ok(T data, string message = "Success") =>
        new() { Success = true, Data = data, Message = message };

    public static CommerceResult<T> Fail(string message) =>
        new() { Success = false, Message = message };
}

public sealed class OrderApiDto
{
    public int OrderId { get; init; }
    public string CustomerEmail { get; init; } = string.Empty;
    public string CustomerName { get; init; } = string.Empty;
    public DateTime OrderDate { get; init; }
    public decimal TotalAmount { get; init; }
    public string Status { get; init; } = string.Empty;
}

public sealed class PlaceOrderRequest
{
    public string ShippingAddress { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string Pincode { get; init; } = string.Empty;
    public string PaymentMethod { get; init; } = "card";
    public IReadOnlyList<PlaceOrderItemRequest> Items { get; init; } = [];
}

public sealed class PlaceOrderItemRequest
{
    public int BookId { get; init; }
    public int Qty { get; init; }
}

public sealed class WishlistApiDto
{
    public int BookId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string AuthorName { get; init; } = string.Empty;
    public string CategoryName { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string ImageUrl { get; init; } = string.Empty;
}

public sealed class ReviewApiDto
{
    public int ReviewId { get; init; }
    public int BookId { get; init; }
    public string UserName { get; init; } = string.Empty;
    public int Rating { get; init; }
    public string Comment { get; init; } = string.Empty;
    public DateTime CreatedUtc { get; init; }
}

public sealed class ProfileApiDto
{
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string Pincode { get; init; } = string.Empty;
}

public sealed class UpdateProfileRequest
{
    public string FullName { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string Pincode { get; init; } = string.Empty;
}

public sealed class ReportSummaryApiDto
{
    public int TotalOrders { get; init; }
    public decimal TotalRevenue { get; init; }
    public int LowStockBooks { get; init; }
    public int TotalBooks { get; init; }
}
