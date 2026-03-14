// Controllers/ProductsController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerceApp.Data;
using ECommerceApp.Models;

namespace ECommerceApp.Controllers;

public class ProductsController : Controller
{
    private readonly ECommerceDbContext _context;

    public ProductsController(ECommerceDbContext context)
    {
        _context = context;
    }

    // GET /Products/Index
    public async Task<IActionResult> Index()
    {
        var products = await _context.Products
            .Include(p => p.Category)
            .ToListAsync();
        return View(products);
    }

    // GET /Products/Details/1
    public async Task<IActionResult> Details(int id)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.ProductId == id);

        if (product == null)
            return NotFound();

        return View(product);
    }

    // GET /Products/Create
    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = await _context.Categories.ToListAsync();
        return View();
    }

    // POST /Products/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product product)
    {
        if (ModelState.IsValid)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        ViewBag.Categories = await _context.Categories.ToListAsync();
        return View(product);
    }

    // GET /Products/Edit/1
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return NotFound();

        ViewBag.Categories = await _context.Categories.ToListAsync();
        return View(product);
    }

    // POST /Products/Edit/1
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Product product)
    {
        if (id != product.ProductId)
            return NotFound();

        if (ModelState.IsValid)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        ViewBag.Categories = await _context.Categories.ToListAsync();
        return View(product);
    }

    // GET /Products/Delete/1
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.ProductId == id);

        if (product == null)
            return NotFound();

        // If redirected here after a failed delete, show a friendly error
        ViewBag.Error = TempData["Error"] as string;

        return View(product);
    }

    // POST /Products/Delete/1
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return RedirectToAction("Index");

        // Prevent deleting a product that has related order details
        bool hasOrderDetails = await _context.OrderDetails.AnyAsync(od => od.ProductId == id);
        if (hasOrderDetails)
        {
            TempData["Error"] = "Cannot delete product because it has related order history.";
            return RedirectToAction("Delete", new { id });
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }
}