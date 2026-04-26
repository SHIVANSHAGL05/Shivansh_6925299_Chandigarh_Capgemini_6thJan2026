using BookStore.API.Data;
using BookStore.API.Dtos;
using BookStore.API.Models;
using BookStore.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.API.Controllers;

[ApiController]
[Route("api/v1/wishlist")]
[Authorize(Roles = "Customer,Admin")]
public class WishlistController : ControllerBase
{
    private readonly BookStoreDbContext _dbContext;
    private readonly BlobStorageService _blobStorageService;

    public WishlistController(BookStoreDbContext dbContext, BlobStorageService blobStorageService)
    {
        _dbContext = dbContext;
        _blobStorageService = blobStorageService;
    }

    [HttpGet("mine")]
    public async Task<IActionResult> Mine(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized(ApiResponse<string>.Fail("Invalid user session."));
        }

        var items = await _dbContext.Wishlists
            .Include(x => x.Book)
            .ThenInclude(x => x!.Author)
            .Include(x => x.Book)
            .ThenInclude(x => x!.Category)
            .Where(x => x.UserId == userId.Value)
            .Select(x => new WishlistItemDto
            {
                BookId = x.BookId,
                Title = x.Book != null ? x.Book.Title : string.Empty,
                AuthorName = x.Book != null && x.Book.Author != null ? x.Book.Author.Name : string.Empty,
                CategoryName = x.Book != null && x.Book.Category != null ? x.Book.Category.Name : string.Empty,
                Price = x.Book != null ? x.Book.Price : 0,
                ImageUrl = x.Book != null ? x.Book.ImageUrl : string.Empty
            })
            .ToListAsync(cancellationToken);

        for (var index = 0; index < items.Count; index++)
        {
            items[index].ImageUrl = await _blobStorageService.GetAccessibleImageUrlAsync(items[index].ImageUrl, cancellationToken);
        }

        return Ok(ApiResponse<IReadOnlyList<WishlistItemDto>>.Ok(items));
    }

    [HttpPost("{bookId:int}")]
    public async Task<IActionResult> Add(int bookId, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized(ApiResponse<string>.Fail("Invalid user session."));
        }

        var exists = await _dbContext.Books.AnyAsync(x => x.BookId == bookId, cancellationToken);
        if (!exists)
        {
            return NotFound(ApiResponse<string>.Fail("Book not found."));
        }

        var duplicate = await _dbContext.Wishlists.AnyAsync(x => x.UserId == userId.Value && x.BookId == bookId, cancellationToken);
        if (duplicate)
        {
            return Ok(ApiResponse<string>.Ok("ok", "Book already in wishlist."));
        }

        _dbContext.Wishlists.Add(new Wishlist { UserId = userId.Value, BookId = bookId });
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponse<string>.Ok("ok", "Book added to wishlist."));
    }

    [HttpDelete("{bookId:int}")]
    public async Task<IActionResult> Remove(int bookId, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized(ApiResponse<string>.Fail("Invalid user session."));
        }

        var record = await _dbContext.Wishlists.FirstOrDefaultAsync(x => x.UserId == userId.Value && x.BookId == bookId, cancellationToken);
        if (record is null)
        {
            return NotFound(ApiResponse<string>.Fail("Wishlist item not found."));
        }

        _dbContext.Wishlists.Remove(record);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Ok(ApiResponse<string>.Ok("ok", "Removed from wishlist."));
    }

    private int? GetUserId()
    {
        var raw = User.FindFirst("user_id")?.Value;
        return int.TryParse(raw, out var userId) ? userId : null;
    }
}
