namespace BookStore.API.Dtos;

public sealed class UserLoginDto
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

public sealed class UserRegisterDto
{
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string Role { get; init; } = "Customer";
    public string? AdminRegistrationKey { get; init; }
    public string Address { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string Pincode { get; init; } = string.Empty;
}

public sealed class AuthResponseDto
{
    public string Token { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
}

public sealed class BookCreateDto
{
    public string Title { get; init; } = string.Empty;
    public string ISBN { get; init; } = string.Empty;
    public string Author { get; init; } = string.Empty;
    public string AuthorName { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string CategoryName { get; init; } = string.Empty;
    public string Publisher { get; init; } = string.Empty;
    public string PublisherName { get; init; } = string.Empty;
    public string ImageUrl { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int Stock { get; init; }
}

public sealed class BookUpdateDto
{
    public string Title { get; init; } = string.Empty;
    public string ISBN { get; init; } = string.Empty;
    public string Author { get; init; } = string.Empty;
    public string AuthorName { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string CategoryName { get; init; } = string.Empty;
    public string Publisher { get; init; } = string.Empty;
    public string PublisherName { get; init; } = string.Empty;
    public string ImageUrl { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int Stock { get; init; }
}

public sealed class BookDto
{
    public int BookId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string ISBN { get; init; } = string.Empty;
    public string Author { get; init; } = string.Empty;
    public string AuthorName { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string CategoryName { get; init; } = string.Empty;
    public string Publisher { get; init; } = string.Empty;
    public string PublisherName { get; init; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public decimal Price { get; init; }
    public int Stock { get; init; }
    public decimal AverageRating { get; init; }
    public int ReviewCount { get; init; }
}

public sealed class ProfileDto
{
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string Pincode { get; init; } = string.Empty;
}

public sealed class UpdateProfileDto
{
    public string FullName { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string Pincode { get; init; } = string.Empty;
}

public sealed class PlaceOrderItemDto
{
    public int BookId { get; init; }
    public int Qty { get; init; }
}

public sealed class PlaceOrderDto
{
    public string ShippingAddress { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string Pincode { get; init; } = string.Empty;
    public string PaymentMethod { get; init; } = "card";
    public IReadOnlyList<PlaceOrderItemDto> Items { get; init; } = [];
}

public sealed class UpdateOrderStatusDto
{
    public string Status { get; init; } = string.Empty;
}

public sealed class OrderActionDto
{
    public string Reason { get; init; } = string.Empty;
}

public sealed class OrderItemDto
{
    public int OrderItemId { get; init; }
    public int BookId { get; init; }
    public string Title { get; init; } = string.Empty;
    public int Qty { get; init; }
    public decimal Price { get; init; }
}

public sealed class OrderDto
{
    public int OrderId { get; init; }
    public string CustomerEmail { get; init; } = string.Empty;
    public string CustomerName { get; init; } = string.Empty;
    public DateTime OrderDate { get; init; }
    public decimal TotalAmount { get; init; }
    public string Status { get; init; } = string.Empty;
    public string ShippingAddress { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string Pincode { get; init; } = string.Empty;
    public string PaymentMethod { get; init; } = string.Empty;
    public IReadOnlyList<OrderItemDto> Items { get; init; } = [];
}

public sealed class ReviewCreateDto
{
    public int Rating { get; init; }
    public string Comment { get; init; } = string.Empty;
}

public sealed class ReviewDto
{
    public int ReviewId { get; init; }
    public int BookId { get; init; }
    public string UserName { get; init; } = string.Empty;
    public int Rating { get; init; }
    public string Comment { get; init; } = string.Empty;
    public DateTime CreatedUtc { get; init; }
}

public sealed class WishlistItemDto
{
    public int BookId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string AuthorName { get; init; } = string.Empty;
    public string CategoryName { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string ImageUrl { get; set; } = string.Empty;
}

public sealed class ReportSummaryDto
{
    public int TotalOrders { get; init; }
    public decimal TotalRevenue { get; init; }
    public int LowStockBooks { get; init; }
    public int TotalBooks { get; init; }
}

public sealed class ApiResponse<T>
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public T? Data { get; init; }

    public static ApiResponse<T> Ok(T data, string message = "Success") =>
        new() { Success = true, Message = message, Data = data };

    public static ApiResponse<T> Fail(string message) =>
        new() { Success = false, Message = message };
}
