using BookStore.Web.Models.ViewModels;
using BookStore.Web.Services.Books;
using BookStore.Web.Services.Commerce;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IBookApiClient _bookApiClient;
    private readonly ICommerceApiClient _commerceApiClient;

    public AdminController(IBookApiClient bookApiClient, ICommerceApiClient commerceApiClient)
    {
        _bookApiClient = bookApiClient;
        _commerceApiClient = commerceApiClient;
    }

    public async Task<IActionResult> Dashboard(CancellationToken cancellationToken)
    {
        var accessToken = User.FindFirst("access_token")?.Value;
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            TempData["AuthMessage"] = "Session expired. Please login again.";
            return RedirectToAction("Login", "Account");
        }

        var booksResult = await _bookApiClient.GetInventoryAsync(accessToken, cancellationToken);
        var ordersResult = await _commerceApiClient.GetAllOrdersAsync(accessToken, cancellationToken);
        var summaryResult = await _commerceApiClient.GetReportSummaryAsync(accessToken, cancellationToken);

        var books = booksResult.Data ?? [];
        var orders = ordersResult.Data ?? [];
        var summary = summaryResult.Data;

        var lowStockItems = books.Where(x => x.Stock <= 5).OrderBy(x => x.Stock).Take(8).ToList();
        var pendingOrders = orders.Count(x => x.Status is "Placed" or "Processing" or "Packed");

        var model = new AdminDashboardViewModel
        {
            Kpis =
            [
                new() { Label = "Revenue (All Time)", Value = "$" + (summary?.TotalRevenue ?? 0).ToString("0.00"), Trend = "Live" },
                new() { Label = "Pending Orders", Value = pendingOrders.ToString(), Trend = "Live" },
                new() { Label = "Low Stock Alerts", Value = lowStockItems.Count.ToString(), Trend = "Live" },
                new() { Label = "Total Books", Value = (summary?.TotalBooks ?? books.Count).ToString(), Trend = "Live" }
            ],
            LowStockItems = lowStockItems,
            RecentOrders = orders
                .OrderByDescending(x => x.OrderDate)
                .Take(5)
                .Select(x => new OrderSummaryViewModel
                {
                    OrderId = x.OrderId.ToString(),
                    Customer = x.CustomerName,
                    OrderedOn = x.OrderDate,
                    Status = x.Status,
                    TotalAmount = x.TotalAmount
                })
                .ToList(),
            Reports =
            [
                new() { Name = "Total Orders", Value = (summary?.TotalOrders ?? orders.Count).ToString(), Delta = "Live" },
                new() { Name = "Low Stock", Value = (summary?.LowStockBooks ?? lowStockItems.Count).ToString(), Delta = "Live" },
                new() { Name = "Pending Orders", Value = pendingOrders.ToString(), Delta = "Live" }
            ]
        };

        if (!booksResult.Success || !ordersResult.Success || !summaryResult.Success)
        {
            TempData["AuthMessage"] = booksResult.Message.Contains("Session expired", StringComparison.OrdinalIgnoreCase) ||
                                       ordersResult.Message.Contains("Session expired", StringComparison.OrdinalIgnoreCase) ||
                                       summaryResult.Message.Contains("Session expired", StringComparison.OrdinalIgnoreCase)
                ? "Session expired. Please login again."
                : "Some dashboard metrics could not be loaded from API.";
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Inventory(CancellationToken cancellationToken)
    {
        var model = await BuildInventoryModelAsync(new CreateBookViewModel(), cancellationToken);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddBook([Bind(Prefix = "CreateBook")] CreateBookViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var invalidModel = await BuildInventoryModelAsync(model, cancellationToken);
            return View("Inventory", invalidModel);
        }

        var accessToken = User.FindFirst("access_token")?.Value;
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            ModelState.AddModelError(string.Empty, "Missing admin session token. Please login again.");
            var tokenErrorModel = await BuildInventoryModelAsync(model, cancellationToken);
            return View("Inventory", tokenErrorModel);
        }

        var imageUrlResult = await ResolveImageUrlAsync(model, accessToken, cancellationToken);
        if (!imageUrlResult.Success)
        {
            ModelState.AddModelError(string.Empty, imageUrlResult.Message);
            var failedImageModel = await BuildInventoryModelAsync(model, cancellationToken);
            return View("Inventory", failedImageModel);
        }

        var createPayload = CloneWithImageUrl(model, imageUrlResult.ImageUrl);
        var createResult = await _bookApiClient.CreateBookAsync(createPayload, accessToken, cancellationToken);
        if (!createResult.Success)
        {
            ModelState.AddModelError(string.Empty, createResult.Message);
            var failedModel = await BuildInventoryModelAsync(model, cancellationToken);
            return View("Inventory", failedModel);
        }

        TempData["AuthMessage"] = "Book added successfully.";
        return RedirectToAction(nameof(Inventory));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateBook(int bookId, CreateBookViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            TempData["AuthMessage"] = "Invalid book payload for update.";
            return RedirectToAction(nameof(Inventory));
        }

        var accessToken = User.FindFirst("access_token")?.Value;
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            TempData["AuthMessage"] = "Session expired. Please login again.";
            return RedirectToAction("Login", "Account");
        }

        var imageUrlResult = await ResolveImageUrlAsync(model, accessToken, cancellationToken);
        if (!imageUrlResult.Success)
        {
            TempData["AuthMessage"] = imageUrlResult.Message;
            return RedirectToAction(nameof(Inventory));
        }

        var updatePayload = CloneWithImageUrl(model, imageUrlResult.ImageUrl);
        var result = await _bookApiClient.UpdateBookAsync(bookId, updatePayload, accessToken, cancellationToken);
        TempData["AuthMessage"] = result.Success ? "Book updated successfully." : result.Message;
        return RedirectToAction(nameof(Inventory));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteBook(int bookId, CancellationToken cancellationToken)
    {
        var accessToken = User.FindFirst("access_token")?.Value;
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            TempData["AuthMessage"] = "Session expired. Please login again.";
            return RedirectToAction("Login", "Account");
        }

        var result = await _bookApiClient.DeleteBookAsync(bookId, accessToken, cancellationToken);
        TempData["AuthMessage"] = result.Success ? "Book deleted successfully." : result.Message;
        return RedirectToAction(nameof(Inventory));
    }

    public async Task<IActionResult> Orders(CancellationToken cancellationToken)
    {
        var accessToken = User.FindFirst("access_token")?.Value;
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            TempData["AuthMessage"] = "Session expired. Please login again.";
            return RedirectToAction("Login", "Account");
        }

        var result = await _commerceApiClient.GetAllOrdersAsync(accessToken, cancellationToken);
        if (!result.Success)
        {
            TempData["AuthMessage"] = result.Message;
            return View(new List<OrderSummaryViewModel>());
        }

        var model = (result.Data ?? [])
            .Select(x => new OrderSummaryViewModel
            {
                OrderId = x.OrderId.ToString(),
                Customer = x.CustomerName,
                OrderedOn = x.OrderDate,
                Status = x.Status,
                TotalAmount = x.TotalAmount
            })
            .ToList();

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateOrderStatus(int orderId, string status, CancellationToken cancellationToken)
    {
        var accessToken = User.FindFirst("access_token")?.Value;
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            TempData["AuthMessage"] = "Session expired. Please login again.";
            return RedirectToAction("Login", "Account");
        }

        var result = await _commerceApiClient.UpdateOrderStatusAsync(orderId, status, accessToken, cancellationToken);
        TempData["AuthMessage"] = result.Success ? "Order status updated." : result.Message;
        return RedirectToAction(nameof(Orders));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelOrder(int orderId, string? reason, CancellationToken cancellationToken)
    {
        var accessToken = User.FindFirst("access_token")?.Value;
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            TempData["AuthMessage"] = "Session expired. Please login again.";
            return RedirectToAction("Login", "Account");
        }

        var result = await _commerceApiClient.CancelOrderAsync(orderId, reason, accessToken, cancellationToken);
        TempData["AuthMessage"] = result.Success ? "Order cancelled." : result.Message;
        return RedirectToAction(nameof(Orders));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ReturnOrder(int orderId, string? reason, CancellationToken cancellationToken)
    {
        var accessToken = User.FindFirst("access_token")?.Value;
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            TempData["AuthMessage"] = "Session expired. Please login again.";
            return RedirectToAction("Login", "Account");
        }

        var result = await _commerceApiClient.ReturnOrderAsync(orderId, reason, accessToken, cancellationToken);
        TempData["AuthMessage"] = result.Success ? "Order return completed." : result.Message;
        return RedirectToAction(nameof(Orders));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RefundOrder(int orderId, string? reason, CancellationToken cancellationToken)
    {
        var accessToken = User.FindFirst("access_token")?.Value;
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            TempData["AuthMessage"] = "Session expired. Please login again.";
            return RedirectToAction("Login", "Account");
        }

        var result = await _commerceApiClient.RefundOrderAsync(orderId, reason, accessToken, cancellationToken);
        TempData["AuthMessage"] = result.Success ? "Order refund marked." : result.Message;
        return RedirectToAction(nameof(Orders));
    }

    public async Task<IActionResult> Reports(CancellationToken cancellationToken)
    {
        var accessToken = User.FindFirst("access_token")?.Value;
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            TempData["AuthMessage"] = "Session expired. Please login again.";
            return RedirectToAction("Login", "Account");
        }

        var summary = await _commerceApiClient.GetReportSummaryAsync(accessToken, cancellationToken);
        if (!summary.Success || summary.Data is null)
        {
            TempData["AuthMessage"] = summary.Message;
            return View(new List<ReportMetricViewModel>());
        }

        var model = new List<ReportMetricViewModel>
        {
            new() { Name = "Total Orders", Value = summary.Data.TotalOrders.ToString(), Delta = "Live" },
            new() { Name = "Total Revenue", Value = "$" + summary.Data.TotalRevenue.ToString("0.00"), Delta = "Live" },
            new() { Name = "Low Stock Books", Value = summary.Data.LowStockBooks.ToString(), Delta = "Live" },
            new() { Name = "Total Books", Value = summary.Data.TotalBooks.ToString(), Delta = "Live" }
        };

        return View(model);
    }

    private async Task<InventoryPageViewModel> BuildInventoryModelAsync(CreateBookViewModel createModel, CancellationToken cancellationToken)
    {
        var accessToken = User.FindFirst("access_token")?.Value;
        var booksResult = await _bookApiClient.GetInventoryAsync(accessToken, cancellationToken);

        if (!booksResult.Success)
        {
            TempData["AuthMessage"] = booksResult.Message;
        }

        return new InventoryPageViewModel
        {
            CreateBook = createModel,
            Items = booksResult.Data ?? []
        };
    }

    private async Task<(bool Success, string Message, string ImageUrl)> ResolveImageUrlAsync(CreateBookViewModel model, string accessToken, CancellationToken cancellationToken)
    {
        var imageUrl = model.ImageUrl?.Trim() ?? string.Empty;
        if (model.ImageFile is null || model.ImageFile.Length == 0)
        {
            return (true, string.Empty, imageUrl);
        }

        var uploadResult = await _bookApiClient.UploadBookImageAsync(model.ImageFile, accessToken, cancellationToken);
        if (!uploadResult.Success || string.IsNullOrWhiteSpace(uploadResult.Data))
        {
            return (false, uploadResult.Message, string.Empty);
        }

        return (true, string.Empty, uploadResult.Data);
    }

    private static CreateBookViewModel CloneWithImageUrl(CreateBookViewModel source, string imageUrl)
    {
        return new CreateBookViewModel
        {
            Title = source.Title,
            Isbn = source.Isbn,
            Author = source.Author,
            Category = source.Category,
            Publisher = source.Publisher,
            ImageUrl = imageUrl,
            Price = source.Price,
            Stock = source.Stock
        };
    }
}
