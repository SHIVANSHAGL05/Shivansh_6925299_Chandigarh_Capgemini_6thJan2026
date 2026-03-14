// Controllers/OrdersController.cs
using ECommerceApp.Data;
using ECommerceApp.Models;
using ECommerceApp.ViewModels;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.ComponentModel;
using System.Diagnostics.Metrics;

namespace ECommerceApp.Controllers;

public class OrdersController : Controller
{
    private readonly ECommerceDbContext _context;

    public OrdersController(ECommerceDbContext context)
    {
        _context = context;
    }

    // GET /Orders/Index
    public async Task<IActionResult> Index()
    {
        var orders = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
            .ToListAsync();

        return View(orders);
    }

    // GET /Orders/Summary/1
    public async Task<IActionResult> Summary(int id)
    {
        var order = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
            .FirstOrDefaultAsync(o => o.OrderId == id);

        if (order == null)
            return NotFound();

        var viewModel = new OrderSummaryViewModel
        {
            Order = order,
            Customer = order.Customer!,
            OrderDetails = order.OrderDetails.ToList()
        };

        return View(viewModel);
    }

    // GET /Orders/Create
    public async Task<IActionResult> Create(int? customerId)
    {
        ViewBag.Customers = await _context.Customers.ToListAsync();
        ViewBag.Products = await _context.Products.ToListAsync();
        ViewBag.CustomerId = customerId; // pre-select the newly created customer
        return View();
    }

    // POST /Orders/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int CustomerId, List<int> ProductIds, List<int> Quantities)
    {
        if (CustomerId == 0 || ProductIds == null || !ProductIds.Any())
        {
            ViewBag.Customers = await _context.Customers.ToListAsync();
            ViewBag.Products = await _context.Products.ToListAsync();
            ViewBag.Error = "Please select a customer and at least one product.";
            return View();
        }

        // Create the order first
        var order = new Order
        {
            CustomerId = CustomerId,
            OrderDate = DateTime.Now,
            TotalAmount = 0
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        // Add order details and calculate total
        decimal total = 0;

        for (int i = 0; i < ProductIds.Count; i++)
        {
            var product = await _context.Products.FindAsync(ProductIds[i]);
            if (product == null) continue;

            int qty = (Quantities != null && i < Quantities.Count) ? Quantities[i] : 1;

            var detail = new OrderDetail
            {
                OrderId = order.OrderId,
                ProductId = ProductIds[i],
                Quantity = qty,
                UnitPrice = product.Price
            };

            total += product.Price * qty;
            _context.OrderDetails.Add(detail);
        }

        // Update total amount
        order.TotalAmount = total;
        await _context.SaveChangesAsync();

        // Redirect to Summary page after placing order
        return RedirectToAction("Summary", new { id = order.OrderId });
    }

    // GET /Orders/Delete/1
    public async Task<IActionResult> Delete(int id)
    {
        var order = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
            .FirstOrDefaultAsync(o => o.OrderId == id);

        if (order == null)
            return NotFound();

        return View(order);
    }

    // POST /Orders/Delete/1
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order != null)
        {
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Index");
    }
}