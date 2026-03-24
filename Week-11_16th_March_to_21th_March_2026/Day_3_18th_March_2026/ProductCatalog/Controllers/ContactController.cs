using Microsoft.AspNetCore.Mvc;

namespace ProductCatalog.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(string name, string email, string message)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(message))
            {
                ViewBag.Error = "All fields are required.";
                return View();
            }
            TempData["Success"] = $"Thanks {name}! We'll get back to you at {email} shortly.";
            return RedirectToAction("Index");
        }
    }
}
