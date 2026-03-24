using Microsoft.AspNetCore.Mvc;
using ShopCart.Services;

namespace ShopCart.Controllers
{
    public class CartController : Controller
    {
        private readonly ShopService _shop;
        public CartController(ShopService shop) => _shop = shop;

        // GET /Cart
        public IActionResult Index()
        {
            var cart = _shop.GetCart(GetSessionId());
            ViewBag.Total     = _shop.CartTotal(GetSessionId());
            ViewBag.CartCount = _shop.CartCount(GetSessionId());
            return View(cart);
        }

        // POST /Cart/Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(int productId, int quantity)
        {
            _shop.UpdateQuantity(GetSessionId(), productId, quantity);
            return RedirectToAction("Index");
        }

        // POST /Cart/Remove
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(int productId)
        {
            _shop.RemoveFromCart(GetSessionId(), productId);
            TempData["Removed"] = "Item removed from cart.";
            return RedirectToAction("Index");
        }

        private string GetSessionId()
        {
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
