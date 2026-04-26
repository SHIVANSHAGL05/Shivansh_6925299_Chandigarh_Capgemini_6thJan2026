using Microsoft.AspNetCore.Mvc;
using ProductMvc.Models;
using ProductMvc.Services;

namespace ProductMvc.Controllers;

public class ProductController : Controller
{
    private readonly ApiService _apiService;

    public ProductController(ApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _apiService.GetProductsAsync();
        return View(products);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product product)
    {
        var (success, errorMessage) = await _apiService.CreateProductAsync(product);
        if (!success)
        {
            ModelState.AddModelError(string.Empty, errorMessage ?? "Failed to create product.");
            return View(product);
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        if (!await _apiService.DeleteProductAsync(id))
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }
}
