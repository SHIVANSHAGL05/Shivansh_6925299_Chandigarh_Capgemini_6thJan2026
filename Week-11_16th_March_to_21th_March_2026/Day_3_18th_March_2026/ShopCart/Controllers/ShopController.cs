using Microsoft.AspNetCore.Mvc;
using ShopCart.Services;

namespace ShopCart.Controllers
{
    public class ShopController : Controller
    {
        private readonly ShopService _shop;
        public ShopController(ShopService shop) => _shop = shop;

        // GET /Shop
        public IActionResult Index(string? category)
        {
            var products = string.IsNullOrEmpty(category)
                ? _shop.GetAllProducts()
                : _shop.GetByCategory(category);

            ViewBag.Categories      = _shop.GetCategories();
            ViewBag.SelectedCategory = category ?? "All";
            ViewBag.CartCount       = _shop.CartCount(GetSessionId());
            return View(products);
        }

        // POST /Shop/AddToCart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddToCart(int productId, string? returnUrl)
        {
            _shop.AddToCart(GetSessionId(), productId);
            TempData["Added"] = "Item added to cart!";
            return Redirect(returnUrl ?? "/Shop");
        }

        private string GetSessionId()
        {
            // Use a simple cookie-based session ID
            if (!Request.Cookies.ContainsKey("shop_session"))
            {
                var id = Guid.NewGuid().ToString();
                Response.Cookies.Append("shop_session", id, new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddDays(1)
                });
                return id;
            }
            return Request.Cookies["shop_session"]!;
        }
    }
}
