using BookStore.Web.Models.ViewModels;

namespace BookStore.Web.Services;

public sealed class StorefrontService : IStorefrontService
{
    private readonly IReadOnlyList<BookCardViewModel> _books = [];
    private readonly List<InternalOrderRecord> _orders = [];
    private readonly object _orderLock = new();

    public HomeDashboardViewModel GetHomeDashboard()
    {
        return new HomeDashboardViewModel
        {
            Kpis =
            [
                new() { Label = "Stores Integrated", Value = "15", Trend = "+3 this quarter" },
                new() { Label = "Books Managed", Value = "0", Trend = "Waiting for API data" },
                new() { Label = "Orders Today", Value = "0", Trend = "Waiting for API data" },
                new() { Label = "Fulfillment SLA", Value = "N/A", Trend = "Waiting for API data" }
            ],
            Workflow =
            [
                new() { Title = "Customer Authentication", Description = "JWT-based login and role-aware experiences for customers and admins." },
                new() { Title = "Catalog to Cart", Description = "Browse, search, and add books while stock and pricing stay synchronized." },
                new() { Title = "Checkout and Payment", Description = "Capture address and payment intent with validation on client and server." },
                new() { Title = "Order and Notification", Description = "Generate order confirmation and low-stock email events through services." },
                new() { Title = "Admin Insights", Description = "Monitor inventory, orders, and revenue metrics from a central dashboard." }
            ],
            TechHighlights =
            [
                "ASP.NET Core MVC (Presentation)",
                "Web API + DTO + AutoMapper (Business Layer)",
                "EF Core + Repository Pattern (Data Layer)",
                "FluentValidation and Regex rules",
                "xUnit + NUnit testing strategy",
                "Azure App Service, SQL, Key Vault, CI/CD"
            ]
        };
    }

    public BookListPageViewModel GetBooks(string? category, string? searchTerm, string? sortBy)
    {
        var normalizedCategory = string.IsNullOrWhiteSpace(category) ? "All" : category;
        var normalizedSearch = searchTerm?.Trim() ?? string.Empty;
        var normalizedSort = string.IsNullOrWhiteSpace(sortBy) ? "featured" : sortBy;

        IEnumerable<BookCardViewModel> filtered = _books;

        if (!string.Equals(normalizedCategory, "All", StringComparison.OrdinalIgnoreCase))
        {
            filtered = filtered.Where(x => x.Category.Equals(normalizedCategory, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(normalizedSearch))
        {
            filtered = filtered.Where(x =>
                x.Title.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase) ||
                x.Author.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase));
        }

        filtered = normalizedSort.ToLowerInvariant() switch
        {
            "price_desc" => filtered.OrderByDescending(x => x.Price),
            "price_asc" => filtered.OrderBy(x => x.Price),
            "rating" => filtered.OrderByDescending(x => x.Rating),
            _ => filtered.OrderByDescending(x => x.Reviews)
        };

        return new BookListPageViewModel
        {
            SearchTerm = normalizedSearch,
            CategoryFilter = normalizedCategory,
            SortBy = normalizedSort,
            Categories = ["All", .. _books.Select(x => x.Category).Distinct().OrderBy(x => x)],
            Books = filtered.ToList()
        };
    }

    public BookCardViewModel? GetBookById(int id)
    {
        return _books.FirstOrDefault(x => x.BookId == id);
    }

    public CartPageViewModel GetCart()
    {
        return new CartPageViewModel { Items = [] };
    }

    public AdminDashboardViewModel GetAdminDashboard()
    {
        return new AdminDashboardViewModel
        {
            Kpis =
            [
                new() { Label = "Revenue (MTD)", Value = "$286K", Trend = "+14.6%" },
                new() { Label = "Pending Orders", Value = "0", Trend = "Waiting for API data" },
                new() { Label = "Low Stock Alerts", Value = "0", Trend = "Waiting for API data" },
                new() { Label = "Avg. Delivery Time", Value = "N/A", Trend = "Waiting for API data" }
            ],
            LowStockItems = GetInventory().Where(x => x.Stock <= 5).ToList(),
            RecentOrders = GetOrders().Take(5).ToList(),
            Reports =
            [
                new() { Name = "Conversion Rate", Value = "0%", Delta = "Waiting for API data" },
                new() { Name = "Average Order Value", Value = "$0", Delta = "Waiting for API data" },
                new() { Name = "Repeat Customers", Value = "0%", Delta = "Waiting for API data" }
            ]
        };
    }

    public IReadOnlyList<InventoryItemViewModel> GetInventory()
    {
        return [];
    }

    public IReadOnlyList<OrderSummaryViewModel> GetOrders()
    {
        lock (_orderLock)
        {
            return _orders
                .OrderByDescending(x => x.OrderedOn)
                .Select(MapOrder)
                .ToList();
        }
    }

    public IReadOnlyList<OrderSummaryViewModel> GetOrdersForUser(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return [];
        }

        lock (_orderLock)
        {
            return _orders
                .Where(x => string.Equals(x.CustomerEmail, email, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(x => x.OrderedOn)
                .Select(MapOrder)
                .ToList();
        }
    }

    public void PlaceOrder(string customerName, string customerEmail, decimal totalAmount)
    {
        if (string.IsNullOrWhiteSpace(customerEmail))
        {
            return;
        }

        var record = new InternalOrderRecord
        {
            OrderId = $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-{Random.Shared.Next(100, 999)}",
            CustomerName = string.IsNullOrWhiteSpace(customerName) ? customerEmail : customerName,
            CustomerEmail = customerEmail,
            OrderedOn = DateTime.UtcNow,
            Status = "Placed",
            TotalAmount = totalAmount
        };

        lock (_orderLock)
        {
            _orders.Add(record);
        }
    }

    private static OrderSummaryViewModel MapOrder(InternalOrderRecord order)
    {
        return new OrderSummaryViewModel
        {
            OrderId = order.OrderId,
            Customer = order.CustomerName,
            OrderedOn = order.OrderedOn,
            Status = order.Status,
            TotalAmount = order.TotalAmount
        };
    }

    private sealed class InternalOrderRecord
    {
        public string OrderId { get; init; } = string.Empty;
        public string CustomerName { get; init; } = string.Empty;
        public string CustomerEmail { get; init; } = string.Empty;
        public DateTime OrderedOn { get; init; }
        public string Status { get; init; } = string.Empty;
        public decimal TotalAmount { get; init; }
    }
}
