using Microsoft.AspNetCore.Mvc;
using EmployeePortal.Models;
using System.Diagnostics;

namespace EmployeePortal.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger) => _logger = logger;

        // Redirect root URL → Admin Login.
        public IActionResult Index() =>
            RedirectToAction("Login", "Account");

        // Used by app.UseExceptionHandler("/Home/Error") in production.
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
            Message   = "An unexpected error occurred. Please try again."
        });
    }
}
