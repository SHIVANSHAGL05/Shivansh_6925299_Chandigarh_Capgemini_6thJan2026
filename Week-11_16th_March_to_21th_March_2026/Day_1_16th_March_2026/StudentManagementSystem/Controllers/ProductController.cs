using Microsoft.AspNetCore.Mvc;
using StudentManagementSystem.Filters;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Controllers
{
    /// <summary>
    /// ProductController — Product Management module.
    ///
    /// Filters applied at class level (cover every action automatically):
    ///   [TypeFilter(typeof(SessionAuthFilter))] — redirects to Login if not logged in
    ///   [TypeFilter(typeof(LogActionFilter))]   — logs action name + timestamp
    ///
    /// Actions:
    ///   Index         — lists all products (hard-coded in-memory list)
    ///   SimulateError — deliberately throws to demonstrate CustomExceptionFilter
    /// </summary>
    [TypeFilter(typeof(SessionAuthFilter))]
    [TypeFilter(typeof(LogActionFilter))]
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;

        public ProductController(ILogger<ProductController> logger)
        {
            _logger = logger;
        }

        // ── GET /Product/Index ────────────────────────────────────────────────
        [HttpGet]
        public IActionResult Index()
        {
            _logger.LogInformation("[ProductController] Index started — loading products.");

            ViewBag.Username = HttpContext.Session.GetString("Username");

            // Hard-coded product list — simulates a product database.
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Wireless Mouse",      Category = "Electronics", Price = 799.00m,  Stock = 50  },
                new Product { Id = 2, Name = "Mechanical Keyboard",  Category = "Electronics", Price = 2499.00m, Stock = 30  },
                new Product { Id = 3, Name = "USB-C Hub",            Category = "Electronics", Price = 1299.00m, Stock = 75  },
                new Product { Id = 4, Name = "Desk Lamp",            Category = "Furniture",   Price = 599.00m,  Stock = 12  },
                new Product { Id = 5, Name = "Notebook A5",          Category = "Stationery",  Price = 120.00m,  Stock = 200 },
                new Product { Id = 6, Name = "Monitor Stand",        Category = "Furniture",   Price = 1850.00m, Stock = 18  },
            };

            _logger.LogInformation("[ProductController] Returning {Count} products.", products.Count);
            return View(products);
        }

        // ── GET /Product/SimulateError ────────────────────────────────────────
        // Deliberately throws an exception.
        // CustomExceptionFilter (registered globally in Program.cs) catches it
        // and renders Views/Shared/Error.cshtml instead of crashing.
        [HttpGet]
        public IActionResult SimulateError()
        {
            _logger.LogInformation("[ProductController] SimulateError — about to throw.");

            throw new InvalidOperationException(
                "This is a deliberate test exception from ProductController.SimulateError " +
                "to demonstrate the CustomExceptionFilter.");
        }
    }
}
