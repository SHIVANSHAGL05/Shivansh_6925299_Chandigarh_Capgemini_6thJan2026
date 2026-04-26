using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Services;

namespace ProductCatalog.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService _service;
        public ProductsController(IProductService service) => _service = service;

        // GET /Products  OR  /Products?q=headphones&category=Audio&sort=price_asc
        public IActionResult Index(string? q, string? category, string? sort)
        {
            ViewBag.Query      = q ?? string.Empty;
            ViewBag.Category   = category ?? "All";
            ViewBag.Sort       = sort ?? "name";
            ViewBag.Categories = _service.GetCategories();

            var products = _service.Search(q ?? string.Empty, category, sort);
            return View(products);
        }

        // GET /Products/Details/5
        public IActionResult Details(int id)
        {
            var product = _service.GetById(id);
            if (product == null) return NotFound();

            // Related products: same category, excluding current
            ViewBag.Related = _service.GetAll()
                .Where(p => p.Category == product.Category && p.Id != id)
                .Take(3);

            return View(product);
        }
    }
}
