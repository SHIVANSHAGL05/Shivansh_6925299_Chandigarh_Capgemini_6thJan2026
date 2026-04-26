using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Services;

namespace ProductCatalog.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _service;
        public HomeController(IProductService service) => _service = service;

        public IActionResult Index()
        {
            ViewBag.Featured   = _service.GetFeatured();
            ViewBag.Categories = _service.GetCategories();
            ViewBag.TotalCount = _service.GetAll().Count();
            return View();
        }
    }
}
