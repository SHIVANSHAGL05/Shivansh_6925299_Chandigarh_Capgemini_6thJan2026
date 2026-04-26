using BookStore.Web.Models.ViewModels;
using BookStore.Web.Services;
using BookStore.Web.Services.Books;
using BookStore.Web.Services.Commerce;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace BookStore.Web.Controllers;

[Authorize(Roles = "Customer,Admin")]
public class CartController : Controller
{
    private const string CartSessionKey = "bookstore_cart_items";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly IStorefrontService _storefrontService;
    private readonly IBookApiClient _bookApiClient;
    private readonly ICommerceApiClient _commerceApiClient;

    public CartController(IStorefrontService storefrontService, IBookApiClient bookApiClient, ICommerceApiClient commerceApiClient)
    {
        _storefrontService = storefrontService;
        _bookApiClient = bookApiClient;
        _commerceApiClient = commerceApiClient;
    }

    public IActionResult Index()
    {
        var model = BuildCartModel();
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Add(int bookId, CancellationToken cancellationToken)
    {
        var accessToken = User.FindFirst("access_token")?.Value;
        var inventoryResult = await _bookApiClient.GetInventoryAsync(accessToken, cancellationToken);
        if (!inventoryResult.Success)
        {
            TempData["AuthMessage"] = inventoryResult.Message;
            return RedirectToAction("Index", "Catalog");
        }

        var selected = (inventoryResult.Data ?? []).FirstOrDefault(x => x.BookId == bookId);
        if (selected is null)
        {
            TempData["AuthMessage"] = "Selected book was not found.";
            return RedirectToAction("Index", "Catalog");
        }

        var items = GetCartItems();
        var existing = items.FirstOrDefault(x => x.BookId == selected.BookId);

        if (existing is null)
        {
            items.Add(new CartSessionItem
            {
                BookId = selected.BookId,
                Title = selected.Title,
                Author = selected.Author,
                UnitPrice = selected.Price,
                Quantity = 1
            });
        }
        else
        {
            existing.Quantity += 1;
        }

        SaveCartItems(items);
        TempData["AuthMessage"] = "Book added to cart.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UpdateQuantity(int bookId, int quantity)
    {
        var items = GetCartItems();
        var target = items.FirstOrDefault(x => x.BookId == bookId);
        if (target is null)
        {
            TempData["AuthMessage"] = "Book not found in cart.";
            return RedirectToAction(nameof(Index));
        }

        if (quantity <= 0)
        {
            items.Remove(target);
        }
        else
        {
            target.Quantity = quantity;
        }

        SaveCartItems(items);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Remove(int bookId)
    {
        var items = GetCartItems();
        var target = items.FirstOrDefault(x => x.BookId == bookId);
        if (target is not null)
        {
            items.Remove(target);
            SaveCartItems(items);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Checkout()
    {
        return View(new CheckoutViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(CheckoutViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var cart = BuildCartModel();
        if (!cart.Items.Any())
        {
            TempData["AuthMessage"] = "Your cart is empty.";
            return RedirectToAction(nameof(Index));
        }

        var accessToken = User.FindFirst("access_token")?.Value;
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            TempData["AuthMessage"] = "Session expired. Please login again.";
            return RedirectToAction("Login", "Account");
        }

        var placeOrder = new PlaceOrderRequest
        {
            ShippingAddress = model.Address,
            City = model.City,
            Pincode = model.PostalCode,
            PaymentMethod = model.PaymentMethod,
            Items = cart.Items.Select(x => new PlaceOrderItemRequest
            {
                BookId = x.BookId,
                Qty = x.Quantity
            }).ToList()
        };

        var result = await _commerceApiClient.PlaceOrderAsync(placeOrder, accessToken, cancellationToken);
        if (!result.Success)
        {
            TempData["AuthMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        TempData["CheckoutName"] = model.FullName;
        HttpContext.Session.Remove(CartSessionKey);
        return RedirectToAction(nameof(Confirmation));
    }

    public IActionResult Confirmation()
    {
        ViewData["CustomerName"] = TempData["CheckoutName"]?.ToString() ?? "Customer";
        return View();
    }

    private CartPageViewModel BuildCartModel()
    {
        var items = GetCartItems()
            .Select(x => new CartItemViewModel
            {
                BookId = x.BookId,
                Title = x.Title,
                Author = x.Author,
                CoverAccent = "sage",
                UnitPrice = x.UnitPrice,
                Quantity = x.Quantity
            })
            .ToList();

        return new CartPageViewModel
        {
            Items = items
        };
    }

    private List<CartSessionItem> GetCartItems()
    {
        var json = HttpContext.Session.GetString(CartSessionKey);
        if (string.IsNullOrWhiteSpace(json))
        {
            return [];
        }

        return JsonSerializer.Deserialize<List<CartSessionItem>>(json, JsonOptions) ?? [];
    }

    private void SaveCartItems(List<CartSessionItem> items)
    {
        var json = JsonSerializer.Serialize(items, JsonOptions);
        HttpContext.Session.SetString(CartSessionKey, json);
    }

    private sealed class CartSessionItem
    {
        public int BookId { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Author { get; init; } = string.Empty;
        public decimal UnitPrice { get; init; }
        public int Quantity { get; set; }
    }
}
