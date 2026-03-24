using Microsoft.AspNetCore.Mvc;
using ProductManagement.Filters;
using ProductManagement.Models;

namespace ProductManagement.Controllers
{
    /// <summary>
    /// ProductController demonstrates:
    ///   1. [TypeFilter(typeof(LogActionFilter))] applied at the controller level
    ///      → every action in this controller is logged automatically.
    ///      TypeFilter resolves LogActionFilter from DI, so ILogger is injected.
    ///   2. Index         — normal action, returns a list of products.
    ///   3. SimulateError — deliberately throws an exception so you can see
    ///      CustomExceptionFilter catch it and render the Error view.
    /// </summary>

    // [TypeFilter(typeof(LogActionFilter))] applied at class level means
    // ALL actions in this controller are logged automatically.
    [TypeFilter(typeof(LogActionFilter))]
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;

        // ILogger is injected via DI — no setup required beyond Program.cs defaults.
        public ProductController(ILogger<ProductController> logger)
        {
            _logger = logger;
        }

        // ─────────────────────────────────────────────────────────────────────
        // GET: /Product/Index
        // Normal action — returns a hard-coded list of products.
        // LogActionFilter will log BEFORE and AFTER this runs.
        // ─────────────────────────────────────────────────────────────────────
        public IActionResult Index()
        {
            _logger.LogInformation("[ProductController] Index action started.");

            // Hard-coded seed data (replace with EF Core DbContext in production).
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Wireless Mouse",    Category = "Electronics", Price = 799.00m,  Stock = 50  },
                new Product { Id = 2, Name = "Mechanical Keyboard",Category = "Electronics", Price = 2499.00m, Stock = 30  },
                new Product { Id = 3, Name = "USB-C Hub",         Category = "Electronics", Price = 1299.00m, Stock = 75  },
                new Product { Id = 4, Name = "Desk Lamp",         Category = "Furniture",   Price = 599.00m,  Stock = 20  },
                new Product { Id = 5, Name = "Notebook A5",       Category = "Stationery",  Price = 120.00m,  Stock = 200 },
            };

            _logger.LogInformation("[ProductController] Returning {Count} products.", products.Count);
            return View(products);
        }

        // ─────────────────────────────────────────────────────────────────────
        // GET: /Product/SimulateError
        // Deliberately throws an exception to demonstrate that
        // CustomExceptionFilter (registered globally) catches it gracefully.
        //
        // Visit this URL in the browser — instead of a crash page you will see
        // the friendly Error view, AND the console will show:
        //   [EXCEPTION FILTER] ✖ Unhandled exception caught | ...
        // ─────────────────────────────────────────────────────────────────────
        public IActionResult SimulateError()
        {
            _logger.LogInformation("[ProductController] SimulateError action started — about to throw.");

            // This exception is intentional for testing purposes.
            throw new InvalidOperationException(
                "This is a test exception thrown from ProductController.SimulateError " +
                "to demonstrate the CustomExceptionFilter.");
        }
    }
}
