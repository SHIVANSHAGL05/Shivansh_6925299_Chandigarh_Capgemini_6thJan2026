using BookStore.API.Data;
using BookStore.API.Dtos;
using BookStore.API.Models;
using BookStore.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.API.Controllers;

[ApiController]
[Route("api/v1/books")]
public class BooksController : ControllerBase
{
    private readonly BookStoreDbContext _dbContext;
    private readonly BlobStorageService _blobStorageService;

    public BooksController(BookStoreDbContext dbContext, BlobStorageService blobStorageService)
    {
        _dbContext = dbContext;
        _blobStorageService = blobStorageService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? category, [FromQuery] string? author, [FromQuery] string? publisher)
    {
        var query = _dbContext.Books
            .Include(x => x.Category)
            .Include(x => x.Author)
            .Include(x => x.Publisher)
            .Include(x => x.Reviews)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(x => x.Category != null && x.Category.Name == category);
        }

        if (!string.IsNullOrWhiteSpace(author))
        {
            query = query.Where(x => x.Author != null && x.Author.Name == author);
        }

        if (!string.IsNullOrWhiteSpace(publisher))
        {
            query = query.Where(x => x.Publisher != null && x.Publisher.Name == publisher);
        }

        var books = await query
            .OrderByDescending(x => x.CreatedUtc)
            .Select(x => new BookDto
            {
                BookId = x.BookId,
                Title = x.Title,
                ISBN = x.ISBN,
                Author = x.Author != null ? x.Author.Name : string.Empty,
                AuthorName = x.Author != null ? x.Author.Name : string.Empty,
                Category = x.Category != null ? x.Category.Name : string.Empty,
                CategoryName = x.Category != null ? x.Category.Name : string.Empty,
                Publisher = x.Publisher != null ? x.Publisher.Name : string.Empty,
                PublisherName = x.Publisher != null ? x.Publisher.Name : string.Empty,
                ImageUrl = x.ImageUrl,
                Price = x.Price,
                Stock = x.Stock,
                AverageRating = x.Reviews.Any() ? x.Reviews.Average(r => (decimal)r.Rating) : 0m,
                ReviewCount = x.Reviews.Count
            })
            .ToListAsync();

        for (var index = 0; index < books.Count; index++)
        {
            books[index].ImageUrl = await _blobStorageService.GetAccessibleImageUrlAsync(books[index].ImageUrl);
        }

        return Ok(ApiResponse<IReadOnlyList<BookDto>>.Ok(books));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] BookCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = string.Join(" ", ModelState.Values
                .SelectMany(x => x.Errors)
                .Select(x => x.ErrorMessage)
                .Where(x => !string.IsNullOrWhiteSpace(x)));
            return BadRequest(ApiResponse<string>.Fail(string.IsNullOrWhiteSpace(errors) ? "Invalid book data." : errors));
        }

        var duplicateIsbn = await _dbContext.Books.AnyAsync(x => x.ISBN == dto.ISBN);
        if (duplicateIsbn)
        {
            return BadRequest(ApiResponse<string>.Fail("ISBN already exists."));
        }

        var category = await EnsureCategoryAsync(string.IsNullOrWhiteSpace(dto.CategoryName) ? dto.Category : dto.CategoryName);
        var author = await EnsureAuthorAsync(string.IsNullOrWhiteSpace(dto.AuthorName) ? dto.Author : dto.AuthorName);
        var publisher = await EnsurePublisherAsync(string.IsNullOrWhiteSpace(dto.PublisherName) ? dto.Publisher : dto.PublisherName);

        var book = new Book
        {
            Title = dto.Title,
            ISBN = dto.ISBN,
            CategoryId = category.CategoryId,
            AuthorId = author.AuthorId,
            PublisherId = publisher.PublisherId,
            ImageUrl = dto.ImageUrl,
            Price = dto.Price,
            Stock = dto.Stock
        };

        _dbContext.Books.Add(book);
        await _dbContext.SaveChangesAsync();

        return Ok(ApiResponse<int>.Ok(book.BookId, "Book created."));
    }

    [HttpPut("{bookId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int bookId, [FromBody] BookUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = string.Join(" ", ModelState.Values
                .SelectMany(x => x.Errors)
                .Select(x => x.ErrorMessage)
                .Where(x => !string.IsNullOrWhiteSpace(x)));
            return BadRequest(ApiResponse<string>.Fail(string.IsNullOrWhiteSpace(errors) ? "Invalid book data." : errors));
        }

        var book = await _dbContext.Books.FirstOrDefaultAsync(x => x.BookId == bookId);
        if (book is null)
        {
            return NotFound(ApiResponse<string>.Fail("Book not found."));
        }

        var duplicateIsbn = await _dbContext.Books.AnyAsync(x => x.ISBN == dto.ISBN && x.BookId != bookId);
        if (duplicateIsbn)
        {
            return BadRequest(ApiResponse<string>.Fail("ISBN already exists."));
        }

        var category = await EnsureCategoryAsync(string.IsNullOrWhiteSpace(dto.CategoryName) ? dto.Category : dto.CategoryName);
        var author = await EnsureAuthorAsync(string.IsNullOrWhiteSpace(dto.AuthorName) ? dto.Author : dto.AuthorName);
        var publisher = await EnsurePublisherAsync(string.IsNullOrWhiteSpace(dto.PublisherName) ? dto.Publisher : dto.PublisherName);

        book.Title = dto.Title;
        book.ISBN = dto.ISBN;
        book.CategoryId = category.CategoryId;
        book.AuthorId = author.AuthorId;
        book.PublisherId = publisher.PublisherId;
        book.ImageUrl = dto.ImageUrl;
        book.Price = dto.Price;
        book.Stock = dto.Stock;

        await _dbContext.SaveChangesAsync();
        return Ok(ApiResponse<int>.Ok(book.BookId, "Book updated."));
    }

    [HttpDelete("{bookId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int bookId)
    {
        var book = await _dbContext.Books
            .Include(x => x.OrderItems)
            .Include(x => x.Reviews)
            .Include(x => x.Wishlists)
            .FirstOrDefaultAsync(x => x.BookId == bookId);

        if (book is null)
        {
            return NotFound(ApiResponse<string>.Fail("Book not found."));
        }

        if (book.OrderItems.Any())
        {
            return BadRequest(ApiResponse<string>.Fail("Cannot delete book with existing order history."));
        }

        _dbContext.Reviews.RemoveRange(book.Reviews);
        _dbContext.Wishlists.RemoveRange(book.Wishlists);
        _dbContext.Books.Remove(book);
        await _dbContext.SaveChangesAsync();

        return Ok(ApiResponse<string>.Ok("ok", "Book deleted."));
    }

    [HttpPost("upload-image")]
    [Authorize(Roles = "Admin")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<IActionResult> UploadImage([FromForm] IFormFile file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest(ApiResponse<string>.Fail("No image file uploaded."));
        }

        var imageUrl = await _blobStorageService.UploadBookImageAsync(file, cancellationToken);
        return Ok(ApiResponse<string>.Ok(imageUrl, "Image uploaded."));
    }

    private async Task<Category> EnsureCategoryAsync(string name)
    {
        var normalized = string.IsNullOrWhiteSpace(name) ? "General" : name.Trim();
        var existing = await _dbContext.Categories.FirstOrDefaultAsync(x => x.Name == normalized);
        if (existing is not null)
        {
            return existing;
        }

        var created = new Category { Name = normalized };
        _dbContext.Categories.Add(created);
        await _dbContext.SaveChangesAsync();
        return created;
    }

    private async Task<Author> EnsureAuthorAsync(string name)
    {
        var normalized = string.IsNullOrWhiteSpace(name) ? "Unknown" : name.Trim();
        var existing = await _dbContext.Authors.FirstOrDefaultAsync(x => x.Name == normalized);
        if (existing is not null)
        {
            return existing;
        }

        var created = new Author { Name = normalized };
        _dbContext.Authors.Add(created);
        await _dbContext.SaveChangesAsync();
        return created;
    }

    private async Task<Publisher> EnsurePublisherAsync(string name)
    {
        var normalized = string.IsNullOrWhiteSpace(name) ? "Unknown" : name.Trim();
        var existing = await _dbContext.Publishers.FirstOrDefaultAsync(x => x.Name == normalized);
        if (existing is not null)
        {
            return existing;
        }

        var created = new Publisher { Name = normalized };
        _dbContext.Publishers.Add(created);
        await _dbContext.SaveChangesAsync();
        return created;
    }
}
