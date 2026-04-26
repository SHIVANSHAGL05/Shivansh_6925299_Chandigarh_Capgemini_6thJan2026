using Microsoft.AspNetCore.Mvc;
using ShopCart.Models;
using ShopCart.Services;

namespace ShopCart.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ShopService _shop;
        public CheckoutController(ShopService shop) => _shop = shop;

        // Step 1 – Address
        // GET /Checkout/Address
        public IActionResult Address()
        {
            var cart = _shop.GetCart(GetSessionId());
            if (!cart.Any())
                return RedirectToAction("Index", "Cart");

            ViewBag.CartCount = _shop.CartCount(GetSessionId());
            ViewBag.Total     = _shop.CartTotal(GetSessionId());
            return View(new Order());
        }

        // POST /Checkout/Address
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Address(Order order)
        {
            // Only validate address fields at this step
            var addressFields = new[] { "FullName", "Email", "Phone", "Address", "City", "PinCode" };
            foreach (var key in ModelState.Keys)
                if (!addressFields.Contains(key))
                    ModelState.Remove(key);

            if (!ModelState.IsValid)
            {
                ViewBag.Total     = _shop.CartTotal(GetSessionId());
                ViewBag.CartCount = _shop.CartCount(GetSessionId());
                return View(order);
            }

            // Save address in TempData to carry to payment step
            TempData["FullName"] = order.FullName;
            TempData["Email"]    = order.Email;
            TempData["Phone"]    = order.Phone;
            TempData["Address"]  = order.Address;
            TempData["City"]     = order.City;
            TempData["PinCode"]  = order.PinCode;

            return RedirectToAction("Payment");
        }

        // Step 2 – Payment
        // GET /Checkout/Payment
        public IActionResult Payment()
        {
            if (TempData["FullName"] == null)
                return RedirectToAction("Address");

            // Keep TempData alive for the POST
            TempData.Keep();

            ViewBag.CartCount = _shop.CartCount(GetSessionId());
            ViewBag.Total     = _shop.CartTotal(GetSessionId());
            return View(new Order());
        }

        // POST /Checkout/Payment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Payment(Order order)
        {
            // Only validate payment fields
            var paymentFields = new[] { "CardNumber", "CardName", "CardExpiry", "CardCvv" };
            foreach (var key in ModelState.Keys)
                if (!paymentFields.Contains(key))
                    ModelState.Remove(key);

            if (!ModelState.IsValid)
            {
                ViewBag.Total     = _shop.CartTotal(GetSessionId());
                ViewBag.CartCount = _shop.CartCount(GetSessionId());
                TempData.Keep();
                return View(order);
            }

            // Restore address from TempData
            order.FullName = TempData["FullName"]?.ToString() ?? "";
            order.Email    = TempData["Email"]?.ToString()    ?? "";
            order.Phone    = TempData["Phone"]?.ToString()    ?? "";
            order.Address  = TempData["Address"]?.ToString()  ?? "";
            order.City     = TempData["City"]?.ToString()     ?? "";
            order.PinCode  = TempData["PinCode"]?.ToString()  ?? "";

            var placed = _shop.PlaceOrder(GetSessionId(), order);
            return RedirectToAction("Confirmation", new { id = placed.Id });
        }

        // Step 3 – Confirmation
        // GET /Checkout/Confirmation/1
        public IActionResult Confirmation(int id)
        {
            var order = _shop.GetOrder(id);
            if (order == null) return NotFound();
            ViewBag.CartCount = 0;
            return View(order);
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
