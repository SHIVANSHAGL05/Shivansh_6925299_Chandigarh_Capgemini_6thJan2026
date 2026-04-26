using BookStore.API.Data;
using BookStore.API.Dtos;
using BookStore.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.API.Controllers;

[ApiController]
[Route("api/v1/books/{bookId:int}/reviews")]
public class ReviewsController : ControllerBase
{
    private readonly BookStoreDbContext _dbContext;

    public ReviewsController(BookStoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetByBook(int bookId, CancellationToken cancellationToken)
    {
        var items = await _dbContext.Reviews
            .Include(x => x.User)
            .Where(x => x.BookId == bookId)
            .OrderByDescending(x => x.CreatedUtc)
            .Select(x => new ReviewDto
            {
                ReviewId = x.ReviewId,
                BookId = x.BookId,
                UserName = x.User != null ? x.User.FullName : string.Empty,
                Rating = x.Rating,
                Comment = x.Comment,
                CreatedUtc = x.CreatedUtc
            })
            .ToListAsync(cancellationToken);

        return Ok(ApiResponse<IReadOnlyList<ReviewDto>>.Ok(items));
    }

    [HttpPost]
    [Authorize(Roles = "Customer,Admin")]
    public async Task<IActionResult> Upsert(int bookId, [FromBody] ReviewCreateDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var errors = string.Join(" ", ModelState.Values
                .SelectMany(x => x.Errors)
                .Select(x => x.ErrorMessage)
                .Where(x => !string.IsNullOrWhiteSpace(x)));
            return BadRequest(ApiResponse<string>.Fail(string.IsNullOrWhiteSpace(errors) ? "Invalid review." : errors));
        }

        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized(ApiResponse<string>.Fail("Invalid user session."));
        }

        var bookExists = await _dbContext.Books.AnyAsync(x => x.BookId == bookId, cancellationToken);
        if (!bookExists)
        {
            return NotFound(ApiResponse<string>.Fail("Book not found."));
        }

        var review = await _dbContext.Reviews.FirstOrDefaultAsync(x => x.UserId == userId.Value && x.BookId == bookId, cancellationToken);
        if (review is null)
        {
            review = new Review
            {
                UserId = userId.Value,
                BookId = bookId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                CreatedUtc = DateTime.UtcNow
            };
            _dbContext.Reviews.Add(review);
        }
        else
        {
            review.Rating = dto.Rating;
            review.Comment = dto.Comment;
            review.CreatedUtc = DateTime.UtcNow;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Ok(ApiResponse<int>.Ok(review.ReviewId, "Review saved."));
    }

    private int? GetUserId()
    {
        var raw = User.FindFirst("user_id")?.Value;
        return int.TryParse(raw, out var userId) ? userId : null;
    }
}
