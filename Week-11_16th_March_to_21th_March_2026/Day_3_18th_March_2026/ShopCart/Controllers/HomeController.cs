using Microsoft.AspNetCore.Mvc;

namespace ShopCart.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Shop");
        }
    }
}
