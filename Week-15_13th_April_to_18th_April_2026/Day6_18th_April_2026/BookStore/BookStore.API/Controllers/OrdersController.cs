using BookStore.API.Data;
using BookStore.API.Dtos;
using BookStore.API.Models;
using BookStore.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.API.Controllers;

[ApiController]
[Route("api/v1/orders")]
[Authorize]
public class OrdersController : ControllerBase
{
    private static readonly HashSet<string> TerminalStatuses =
    [
        "Cancelled", "Refunded"
    ];

    private readonly BookStoreDbContext _dbContext;
    private readonly EmailNotificationService _emailNotificationService;
    private readonly IConfiguration _configuration;

    public OrdersController(BookStoreDbContext dbContext, EmailNotificationService emailNotificationService, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _emailNotificationService = emailNotificationService;
        _configuration = configuration;
    }

    [HttpPost]
    [Authorize(Roles = "Customer,Admin")]
    public async Task<IActionResult> Place([FromBody] PlaceOrderDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var errors = string.Join(" ", ModelState.Values
                .SelectMany(x => x.Errors)
                .Select(x => x.ErrorMessage)
                .Where(x => !string.IsNullOrWhiteSpace(x)));
            return BadRequest(ApiResponse<string>.Fail(string.IsNullOrWhiteSpace(errors) ? "Invalid order request." : errors));
        }

        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized(ApiResponse<string>.Fail("Invalid user session."));
        }

        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.AppUserId == userId.Value, cancellationToken);
        if (user is null)
        {
            return Unauthorized(ApiResponse<string>.Fail("User not found."));
        }

        var bookIds = dto.Items.Select(x => x.BookId).Distinct().ToList();
        var books = await _dbContext.Books.Where(x => bookIds.Contains(x.BookId)).ToDictionaryAsync(x => x.BookId, cancellationToken);

        foreach (var item in dto.Items)
        {
            if (!books.TryGetValue(item.BookId, out var book))
            {
                return BadRequest(ApiResponse<string>.Fail($"Book {item.BookId} not found."));
            }

            if (book.Stock < item.Qty)
            {
                return BadRequest(ApiResponse<string>.Fail($"Insufficient stock for '{book.Title}'."));
            }
        }

        var order = new Order
        {
            UserId = user.AppUserId,
            OrderDate = DateTime.UtcNow,
            Status = "Placed",
            ShippingAddress = dto.ShippingAddress,
            ShippingCity = dto.City,
            ShippingPincode = dto.Pincode,
            PaymentMethod = dto.PaymentMethod
        };

        foreach (var item in dto.Items)
        {
            var book = books[item.BookId];
            order.Items.Add(new OrderItem
            {
                BookId = book.BookId,
                Qty = item.Qty,
                Price = book.Price
            });

            book.Stock -= item.Qty;
        }

        order.TotalAmount = order.Items.Sum(x => x.Price * x.Qty);

        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _emailNotificationService.LogOrderConfirmationAsync(user.Email, order.OrderId, cancellationToken);
        await _emailNotificationService.LogInvoiceAsync(user.Email, order.OrderId, cancellationToken);

        var lowStockBooks = books.Values.Where(x => x.Stock <= 5).ToList();
        if (lowStockBooks.Count > 0)
        {
            var adminEmail = _configuration["Notifications:AdminEmail"];
            if (string.IsNullOrWhiteSpace(adminEmail))
            {
                adminEmail = await _dbContext.Users
                    .Where(x => x.Role == "Admin")
                    .Select(x => x.Email)
                    .FirstOrDefaultAsync(cancellationToken);
            }

            if (!string.IsNullOrWhiteSpace(adminEmail))
            {
                foreach (var lowStockBook in lowStockBooks)
                {
                    await _emailNotificationService.LogLowStockAsync(adminEmail, lowStockBook.Title, lowStockBook.Stock, cancellationToken);
                }
            }
        }

        return Ok(ApiResponse<int>.Ok(order.OrderId, "Order placed."));
    }

    [HttpGet("mine")]
    [Authorize(Roles = "Customer,Admin")]
    public async Task<IActionResult> Mine(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized(ApiResponse<string>.Fail("Invalid user session."));
        }

        var data = await BuildOrdersQuery(userId.Value).ToListAsync(cancellationToken);

        return Ok(ApiResponse<IReadOnlyList<OrderDto>>.Ok(data));
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> All(CancellationToken cancellationToken)
    {
        var data = await BuildOrdersQuery().ToListAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<OrderDto>>.Ok(data));
    }

    [HttpPatch("{orderId:int}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus(int orderId, [FromBody] UpdateOrderStatusDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var errors = string.Join(" ", ModelState.Values
                .SelectMany(x => x.Errors)
                .Select(x => x.ErrorMessage)
                .Where(x => !string.IsNullOrWhiteSpace(x)));
            return BadRequest(ApiResponse<string>.Fail(string.IsNullOrWhiteSpace(errors) ? "Invalid status request." : errors));
        }

        var order = await _dbContext.Orders.Include(x => x.Items).FirstOrDefaultAsync(x => x.OrderId == orderId, cancellationToken);
        if (order is null)
        {
            return NotFound(ApiResponse<string>.Fail("Order not found."));
        }

        var normalizedTarget = NormalizeStatus(dto.Status);
        if (!IsTransitionAllowed(order.Status, normalizedTarget))
        {
            return BadRequest(ApiResponse<string>.Fail($"Cannot transition from '{order.Status}' to '{normalizedTarget}'."));
        }

        order.Status = normalizedTarget;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Ok(ApiResponse<string>.Ok("ok", "Order status updated."));
    }

    [HttpPost("{orderId:int}/cancel")]
    [Authorize(Roles = "Customer,Admin")]
    public async Task<IActionResult> Cancel(int orderId, [FromBody] OrderActionDto? dto, CancellationToken cancellationToken)
    {
        var currentUserId = GetUserId();
        if (currentUserId is null)
        {
            return Unauthorized(ApiResponse<string>.Fail("Invalid user session."));
        }

        var order = await _dbContext.Orders
            .Include(x => x.Items)
            .ThenInclude(x => x.Book)
            .FirstOrDefaultAsync(x => x.OrderId == orderId, cancellationToken);

        if (order is null)
        {
            return NotFound(ApiResponse<string>.Fail("Order not found."));
        }

        if (!User.IsInRole("Admin") && order.UserId != currentUserId.Value)
        {
            return Forbid();
        }

        if (!IsTransitionAllowed(order.Status, "Cancelled"))
        {
            return BadRequest(ApiResponse<string>.Fail($"Order cannot be cancelled in '{order.Status}' state."));
        }

        foreach (var item in order.Items)
        {
            if (item.Book is not null)
            {
                item.Book.Stock += item.Qty;
            }
        }

        order.Status = "Cancelled";
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Ok(ApiResponse<string>.Ok("ok", "Order cancelled."));
    }

    [HttpPost("{orderId:int}/return")]
    [Authorize(Roles = "Customer,Admin")]
    public async Task<IActionResult> Return(int orderId, [FromBody] OrderActionDto? dto, CancellationToken cancellationToken)
    {
        var currentUserId = GetUserId();
        if (currentUserId is null)
        {
            return Unauthorized(ApiResponse<string>.Fail("Invalid user session."));
        }

        var order = await _dbContext.Orders
            .Include(x => x.Items)
            .ThenInclude(x => x.Book)
            .FirstOrDefaultAsync(x => x.OrderId == orderId, cancellationToken);

        if (order is null)
        {
            return NotFound(ApiResponse<string>.Fail("Order not found."));
        }

        if (!User.IsInRole("Admin") && order.UserId != currentUserId.Value)
        {
            return Forbid();
        }

        if (!string.Equals(order.Status, "Delivered", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(ApiResponse<string>.Fail("Only delivered orders can be returned."));
        }

        foreach (var item in order.Items)
        {
            if (item.Book is not null)
            {
                item.Book.Stock += item.Qty;
            }
        }

        order.Status = "Returned";
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Ok(ApiResponse<string>.Ok("ok", "Return completed."));
    }

    [HttpPost("{orderId:int}/refund")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Refund(int orderId, [FromBody] OrderActionDto? dto, CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders.FirstOrDefaultAsync(x => x.OrderId == orderId, cancellationToken);
        if (order is null)
        {
            return NotFound(ApiResponse<string>.Fail("Order not found."));
        }

        if (!string.Equals(order.Status, "Cancelled", StringComparison.OrdinalIgnoreCase)
            && !string.Equals(order.Status, "Returned", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(ApiResponse<string>.Fail("Refund is only allowed for cancelled or returned orders."));
        }

        order.Status = "Refunded";
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Ok(ApiResponse<string>.Ok("ok", "Refund marked as completed."));
    }

    private IQueryable<OrderDto> BuildOrdersQuery(int? userId = null)
    {
        var query = _dbContext.Orders
            .Include(x => x.User)
            .Include(x => x.Items)
            .ThenInclude(x => x.Book)
            .AsQueryable();

        if (userId.HasValue)
        {
            query = query.Where(x => x.UserId == userId.Value);
        }

        return query
            .OrderByDescending(x => x.OrderDate)
            .Select(x => new OrderDto
            {
                OrderId = x.OrderId,
                CustomerName = x.User != null ? x.User.FullName : string.Empty,
                CustomerEmail = x.User != null ? x.User.Email : string.Empty,
                OrderDate = x.OrderDate,
                TotalAmount = x.TotalAmount,
                Status = x.Status,
                ShippingAddress = x.ShippingAddress,
                City = x.ShippingCity,
                Pincode = x.ShippingPincode,
                PaymentMethod = x.PaymentMethod,
                Items = x.Items.Select(i => new OrderItemDto
                {
                    OrderItemId = i.OrderItemId,
                    BookId = i.BookId,
                    Title = i.Book != null ? i.Book.Title : string.Empty,
                    Qty = i.Qty,
                    Price = i.Price
                }).ToList()
            });
    }

    private int? GetUserId()
    {
        var raw = User.FindFirst("user_id")?.Value;
        return int.TryParse(raw, out var userId) ? userId : null;
    }

    private static string NormalizeStatus(string status)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            return "Placed";
        }

        return status.Trim().ToLowerInvariant() switch
        {
            "placed" => "Placed",
            "processing" => "Processing",
            "packed" => "Packed",
            "shipped" => "Shipped",
            "delivered" => "Delivered",
            "cancelled" => "Cancelled",
            "returnrequested" => "ReturnRequested",
            "returned" => "Returned",
            "refunded" => "Refunded",
            _ => status.Trim()
        };
    }

    private static bool IsTransitionAllowed(string current, string target)
    {
        var normalizedCurrent = NormalizeStatus(current);
        var normalizedTarget = NormalizeStatus(target);

        if (string.Equals(normalizedCurrent, normalizedTarget, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (TerminalStatuses.Contains(normalizedCurrent))
        {
            return false;
        }

        return normalizedCurrent switch
        {
            "Placed" => normalizedTarget is "Processing" or "Cancelled",
            "Processing" => normalizedTarget is "Packed" or "Cancelled",
            "Packed" => normalizedTarget is "Shipped" or "Cancelled",
            "Shipped" => normalizedTarget == "Delivered",
            "Delivered" => normalizedTarget == "Returned",
            "Returned" => normalizedTarget == "Refunded",
            _ => false
        };
    }
}
