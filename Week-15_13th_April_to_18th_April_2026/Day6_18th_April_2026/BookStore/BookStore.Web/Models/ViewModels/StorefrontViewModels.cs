using System.ComponentModel.DataAnnotations;

namespace BookStore.Web.Models.ViewModels;

public sealed class HomeDashboardViewModel
{
    public required IReadOnlyList<PlatformKpiViewModel> Kpis { get; init; }
    public required IReadOnlyList<WorkflowStepViewModel> Workflow { get; init; }
    public required IReadOnlyList<string> TechHighlights { get; init; }
}

public sealed class PlatformKpiViewModel
{
    public required string Label { get; init; }
    public required string Value { get; init; }
    public required string Trend { get; init; }
}

public sealed class WorkflowStepViewModel
{
    public required string Title { get; init; }
    public required string Description { get; init; }
}

public sealed class BookListPageViewModel
{
    public string SearchTerm { get; init; } = string.Empty;
    public string CategoryFilter { get; init; } = "All";
    public string SortBy { get; init; } = "featured";
    public required IReadOnlyList<string> Categories { get; init; }
    public required IReadOnlyList<BookCardViewModel> Books { get; init; }
}

public sealed class BookCardViewModel
{
    public int BookId { get; init; }
    public required string Title { get; init; }
    public required string Author { get; init; }
    public required string Category { get; init; }
    public string Publisher { get; init; } = string.Empty;
    public required string Isbn { get; init; }
    public decimal Price { get; init; }
    public int Stock { get; init; }
    public decimal Rating { get; init; }
    public int Reviews { get; init; }
    public required string CoverAccent { get; init; }
    public string ImageUrl { get; init; } = string.Empty;
    public bool IsLowStock => Stock <= 5;
}

public sealed class CartPageViewModel
{
    public required IReadOnlyList<CartItemViewModel> Items { get; init; }
    public decimal Subtotal => Items.Sum(i => i.LineTotal);
    public decimal Tax => Subtotal * 0.08m;
    public decimal Shipping => Subtotal >= 75 ? 0 : 7.99m;
    public decimal GrandTotal => Subtotal + Tax + Shipping;
}

public sealed class CartItemViewModel
{
    public int BookId { get; init; }
    public required string Title { get; init; }
    public required string Author { get; init; }
    public required string CoverAccent { get; init; }
    public decimal UnitPrice { get; init; }
    public int Quantity { get; init; }
    public decimal LineTotal => UnitPrice * Quantity;
}

public sealed class CheckoutViewModel
{
    [Required]
    [Display(Name = "Full Name")]
    public string FullName { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required]
    [Display(Name = "Phone Number")]
    public string Phone { get; init; } = string.Empty;

    [Required]
    [Display(Name = "Shipping Address")]
    public string Address { get; init; } = string.Empty;

    [Required]
    public string City { get; init; } = string.Empty;

    [Required]
    [Display(Name = "Postal Code")]
    public string PostalCode { get; init; } = string.Empty;

    [Required]
    [Display(Name = "Payment Method")]
    public string PaymentMethod { get; init; } = "card";
}

public sealed class LoginViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; init; } = string.Empty;

    [Display(Name = "Remember Me")]
    public bool RememberMe { get; init; }
}

public sealed class RegisterViewModel
{
    [Required]
    [Display(Name = "Full Name")]
    public string FullName { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required]
    [Display(Name = "Phone Number")]
    public string Phone { get; init; } = string.Empty;

    [Required]
    [Display(Name = "Address")]
    public string Address { get; init; } = string.Empty;

    [Required]
    [Display(Name = "City")]
    public string City { get; init; } = string.Empty;

    [Required]
    [Display(Name = "Pincode")]
    public string Pincode { get; init; } = string.Empty;

    [Required]
    public string Role { get; init; } = "Customer";

    [Display(Name = "Admin Registration Key")]
    public string? AdminRegistrationKey { get; init; }

    [Required]
    [DataType(DataType.Password)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\w\s]).{8,}$",
        ErrorMessage = "Password must include uppercase, lowercase, number, and symbol.")]
    public string Password { get; init; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password")]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; init; } = string.Empty;
}

public sealed class AdminDashboardViewModel
{
    public required IReadOnlyList<PlatformKpiViewModel> Kpis { get; init; }
    public required IReadOnlyList<InventoryItemViewModel> LowStockItems { get; init; }
    public required IReadOnlyList<OrderSummaryViewModel> RecentOrders { get; init; }
    public required IReadOnlyList<ReportMetricViewModel> Reports { get; init; }
}

public sealed class InventoryItemViewModel
{
    public int BookId { get; init; }
    public required string Title { get; init; }
    public required string Author { get; init; }
    public required string Isbn { get; init; }
    public required string Category { get; init; }
    public required string Publisher { get; init; }
    public int Stock { get; init; }
    public decimal Price { get; init; }
    public string ImageUrl { get; init; } = string.Empty;
    public decimal AverageRating { get; init; }
    public int ReviewCount { get; init; }
}

public sealed class CreateBookViewModel
{
    [Required]
    [Display(Name = "Title")]
    public string Title { get; init; } = string.Empty;

    [Required]
    [Display(Name = "ISBN")]
    public string Isbn { get; init; } = string.Empty;

    [Required]
    [Display(Name = "Author")]
    public string Author { get; init; } = string.Empty;

    [Required]
    [Display(Name = "Category")]
    public string Category { get; init; } = string.Empty;

    [Required]
    [Display(Name = "Publisher")]
    public string Publisher { get; init; } = string.Empty;

    [Display(Name = "Image URL")]
    public string ImageUrl { get; init; } = string.Empty;

    [Display(Name = "Upload Image")]
    public IFormFile? ImageFile { get; init; }

    [Range(0.01, 999999)]
    [Display(Name = "Price")]
    public decimal Price { get; init; }

    [Range(0, int.MaxValue)]
    [Display(Name = "Stock")]
    public int Stock { get; init; }
}

public sealed class InventoryPageViewModel
{
    public required CreateBookViewModel CreateBook { get; init; }
    public required IReadOnlyList<InventoryItemViewModel> Items { get; init; }
}

public sealed class OrderSummaryViewModel
{
    public required string OrderId { get; init; }
    public required string Customer { get; init; }
    public required DateTime OrderedOn { get; init; }
    public required string Status { get; init; }
    public decimal TotalAmount { get; init; }
}

public sealed class ProfileViewModel
{
    [Required]
    [Display(Name = "Full Name")]
    public string FullName { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required]
    public string Phone { get; init; } = string.Empty;

    [Required]
    [Display(Name = "Address")]
    public string Address { get; init; } = string.Empty;

    [Required]
    [Display(Name = "City")]
    public string City { get; init; } = string.Empty;

    [Required]
    [Display(Name = "Pincode")]
    public string Pincode { get; init; } = string.Empty;
}

public sealed class BookReviewViewModel
{
    public string UserName { get; init; } = string.Empty;
    public int Rating { get; init; }
    public string Comment { get; init; } = string.Empty;
    public DateTime CreatedUtc { get; init; }
}

public sealed class AddReviewViewModel
{
    public int BookId { get; init; }

    [Range(1, 5)]
    public int Rating { get; init; } = 5;

    [Required]
    [MaxLength(800)]
    public string Comment { get; init; } = string.Empty;
}

public sealed class BookDetailsPageViewModel
{
    public required BookCardViewModel Book { get; init; }
    public required IReadOnlyList<BookReviewViewModel> Reviews { get; init; }
    public required AddReviewViewModel AddReview { get; init; }
}

public sealed class ReportMetricViewModel
{
    public required string Name { get; init; }
    public required string Value { get; init; }
    public required string Delta { get; init; }
}
