using BookStore.Web.Services;
using BookStore.Web.Services.Books;
using BookStore.Web.Services.Commerce;
using BookStore.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Web.Controllers;

[Authorize(Roles = "Customer,Admin")]
public class CatalogController : Controller
{
    private readonly IStorefrontService _storefrontService;
    private readonly IBookApiClient _bookApiClient;
    private readonly ICommerceApiClient _commerceApiClient;

    public CatalogController(IStorefrontService storefrontService, IBookApiClient bookApiClient, ICommerceApiClient commerceApiClient)
    {
        _storefrontService = storefrontService;
        _bookApiClient = bookApiClient;
        _commerceApiClient = commerceApiClient;
    }

    public async Task<IActionResult> Index(string? category, string? author, string? publisher, string? searchTerm, string? sortBy, CancellationToken cancellationToken)
    {
        var accessToken = User.FindFirst("access_token")?.Value;
        var inventoryResult = await _bookApiClient.GetInventoryAsync(accessToken, cancellationToken);

        if (!inventoryResult.Success)
        {
            TempData["AuthMessage"] = inventoryResult.Message;
        }

        var allBooks = (inventoryResult.Data ?? [])
            .Select(x => new BookCardViewModel
            {
                BookId = x.BookId,
                Title = x.Title,
                Author = x.Author,
                Category = x.Category,
                Publisher = x.Publisher,
                Isbn = x.Isbn,
                Price = x.Price,
                Stock = x.Stock,
                Rating = x.AverageRating,
                Reviews = x.ReviewCount,
                CoverAccent = "sage",
                ImageUrl = x.ImageUrl
            })
            .ToList();

        var normalizedCategory = string.IsNullOrWhiteSpace(category) ? "All" : category;
        var normalizedAuthor = string.IsNullOrWhiteSpace(author) ? "All" : author;
        var normalizedPublisher = string.IsNullOrWhiteSpace(publisher) ? "All" : publisher;
        var normalizedSearch = searchTerm?.Trim() ?? string.Empty;
        var normalizedSort = string.IsNullOrWhiteSpace(sortBy) ? "featured" : sortBy;

        IEnumerable<BookCardViewModel> filtered = allBooks;

        if (!string.Equals(normalizedCategory, "All", StringComparison.OrdinalIgnoreCase))
        {
            filtered = filtered.Where(x => x.Category.Equals(normalizedCategory, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.Equals(normalizedAuthor, "All", StringComparison.OrdinalIgnoreCase))
        {
            filtered = filtered.Where(x => x.Author.Equals(normalizedAuthor, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.Equals(normalizedPublisher, "All", StringComparison.OrdinalIgnoreCase))
        {
            filtered = filtered.Where(x => x.Publisher.Equals(normalizedPublisher, StringComparison.OrdinalIgnoreCase));
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
            _ => filtered.OrderBy(x => x.Title)
        };

        var model = new BookListPageViewModel
        {
            SearchTerm = normalizedSearch,
            CategoryFilter = normalizedCategory,
            SortBy = normalizedSort,
            Categories = ["All", .. allBooks.Select(x => x.Category).Distinct().OrderBy(x => x)],
            Books = filtered.ToList()
        };

        return View(model);
    }

    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var accessToken = User.FindFirst("access_token")?.Value;
        var inventoryResult = await _bookApiClient.GetInventoryAsync(accessToken, cancellationToken);
        var book = (inventoryResult.Data ?? [])
            .Where(x => x.BookId == id)
            .Select(x => new BookCardViewModel
            {
                BookId = x.BookId,
                Title = x.Title,
                Author = x.Author,
                Category = x.Category,
                Publisher = x.Publisher,
                Isbn = x.Isbn,
                Price = x.Price,
                Stock = x.Stock,
                Rating = x.AverageRating,
                Reviews = x.ReviewCount,
                CoverAccent = "sage",
                ImageUrl = x.ImageUrl
            })
            .FirstOrDefault();

        if (book is null)
        {
            return NotFound();
        }

        var reviewsResult = await _commerceApiClient.GetReviewsAsync(id, cancellationToken);
        var reviews = (reviewsResult.Data ?? [])
            .Select(x => new BookReviewViewModel
            {
                UserName = x.UserName,
                Rating = x.Rating,
                Comment = x.Comment,
                CreatedUtc = x.CreatedUtc
            })
            .ToList();

        var model = new BookDetailsPageViewModel
        {
            Book = book,
            Reviews = reviews,
            AddReview = new AddReviewViewModel { BookId = id }
        };

        return View(model);
    }

    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> Orders(CancellationToken cancellationToken)
    {
        var accessToken = User.FindFirst("access_token")?.Value;
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            TempData["AuthMessage"] = "Session expired. Please login again.";
            return RedirectToAction("Login", "Account");
        }

        var result = await _commerceApiClient.GetMyOrdersAsync(accessToken, cancellationToken);
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

    [Authorize(Roles = "Customer,Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddReview(AddReviewViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            TempData["AuthMessage"] = "Invalid review payload.";
            return RedirectToAction(nameof(Details), new { id = model.BookId });
        }

        var accessToken = User.FindFirst("access_token")?.Value;
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            TempData["AuthMessage"] = "Session expired. Please login again.";
            return RedirectToAction("Login", "Account");
        }

        var result = await _commerceApiClient.UpsertReviewAsync(model.BookId, model.Rating, model.Comment, accessToken, cancellationToken);
        TempData["AuthMessage"] = result.Success ? "Review saved." : result.Message;
        return RedirectToAction(nameof(Details), new { id = model.BookId });
    }

    [Authorize(Roles = "Customer,Admin")]
    public async Task<IActionResult> Wishlist(CancellationToken cancellationToken)
    {
        var accessToken = User.FindFirst("access_token")?.Value;
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            TempData["AuthMessage"] = "Session expired. Please login again.";
            return RedirectToAction("Login", "Account");
        }

        var result = await _commerceApiClient.GetWishlistAsync(accessToken, cancellationToken);
        if (!result.Success)
        {
            TempData["AuthMessage"] = result.Message;
            return View(new List<BookCardViewModel>());
        }

        var model = (result.Data ?? [])
            .Select(x => new BookCardViewModel
            {
                BookId = x.BookId,
                Title = x.Title,
                Author = x.AuthorName,
                Category = x.CategoryName,
                Publisher = string.Empty,
                Isbn = string.Empty,
                Price = x.Price,
                Stock = 0,
                Rating = 0,
                Reviews = 0,
                CoverAccent = "sage",
                ImageUrl = x.ImageUrl
            })
            .ToList();

        return View(model);
    }

    [Authorize(Roles = "Customer,Admin")]
    public async Task<IActionResult> AddWishlist(int bookId, CancellationToken cancellationToken)
    {
        var accessToken = User.FindFirst("access_token")?.Value;
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            TempData["AuthMessage"] = "Session expired. Please login again.";
            return RedirectToAction("Login", "Account");
        }

        var result = await _commerceApiClient.AddWishlistAsync(bookId, accessToken, cancellationToken);
        TempData["AuthMessage"] = result.Success ? "Book added to wishlist." : result.Message;
        return RedirectToAction(nameof(Details), new { id = bookId });
    }

    [Authorize(Roles = "Customer,Admin")]
    public async Task<IActionResult> RemoveWishlist(int bookId, CancellationToken cancellationToken)
    {
        var accessToken = User.FindFirst("access_token")?.Value;
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            TempData["AuthMessage"] = "Session expired. Please login again.";
            return RedirectToAction("Login", "Account");
        }

        var result = await _commerceApiClient.RemoveWishlistAsync(bookId, accessToken, cancellationToken);
        TempData["AuthMessage"] = result.Success ? "Removed from wishlist." : result.Message;
        return RedirectToAction(nameof(Wishlist));
    }
}
