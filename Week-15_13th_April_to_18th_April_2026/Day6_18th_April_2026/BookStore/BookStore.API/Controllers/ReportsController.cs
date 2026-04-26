using BookStore.API.Data;
using BookStore.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.API.Controllers;

[ApiController]
[Route("api/v1/reports")]
[Authorize(Roles = "Admin")]
public class ReportsController : ControllerBase
{
    private readonly BookStoreDbContext _dbContext;

    public ReportsController(BookStoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> Summary(CancellationToken cancellationToken)
    {
        var totalOrders = await _dbContext.Orders.CountAsync(cancellationToken);
        var totalRevenue = await _dbContext.Orders.SumAsync(x => (decimal?)x.TotalAmount, cancellationToken) ?? 0;
        var lowStockBooks = await _dbContext.Books.CountAsync(x => x.Stock <= 5, cancellationToken);
        var totalBooks = await _dbContext.Books.CountAsync(cancellationToken);

        var data = new ReportSummaryDto
        {
            TotalOrders = totalOrders,
            TotalRevenue = totalRevenue,
            LowStockBooks = lowStockBooks,
            TotalBooks = totalBooks
        };

        return Ok(ApiResponse<ReportSummaryDto>.Ok(data));
    }
}
