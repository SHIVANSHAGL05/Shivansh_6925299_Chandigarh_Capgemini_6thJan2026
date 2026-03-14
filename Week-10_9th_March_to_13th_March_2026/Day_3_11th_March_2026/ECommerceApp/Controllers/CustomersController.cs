// Controllers/CustomersController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerceApp.Data;
using ECommerceApp.Models;

namespace ECommerceApp.Controllers;

public class CustomersController : Controller
{
    private readonly ECommerceDbContext _context;

    public CustomersController(ECommerceDbContext context)
    {
        _context = context;
    }

    // GET /Customers/Index
    public async Task<IActionResult> Index()
    {
        var customers = await _context.Customers
            .Include(c => c.Orders)
            .ToListAsync();
        return View(customers);
    }

    // GET /Customers/Details/1
    public async Task<IActionResult> Details(int id)
    {
        var customer = await _context.Customers
            .Include(c => c.Orders)
                .ThenInclude(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
            .FirstOrDefaultAsync(c => c.CustomerId == id);

        if (customer == null)
            return NotFound();

        return View(customer);
    }

    // GET /Customers/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST /Customers/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Customer customer)
    {
        if (ModelState.IsValid)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            // After creating customer, redirect to place an order for them
            return RedirectToAction("Create", "Orders", new { customerId = customer.CustomerId });
        }
        return View(customer);
    }

    // GET /Customers/Edit/1
    public async Task<IActionResult> Edit(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null)
            return NotFound();
        return View(customer);
    }

    // POST /Customers/Edit/1
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Customer customer)
    {
        if (id != customer.CustomerId)
            return NotFound();

        if (ModelState.IsValid)
        {
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        return View(customer);
    }

    // GET /Customers/Delete/1
    public async Task<IActionResult> Delete(int id)
    {
        var customer = await _context.Customers
            .Include(c => c.Orders)
            .FirstOrDefaultAsync(c => c.CustomerId == id);

        if (customer == null)
            return NotFound();

        return View(customer);
    }

    // POST /Customers/Delete/1
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer != null)
        {
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Index");
    }
}