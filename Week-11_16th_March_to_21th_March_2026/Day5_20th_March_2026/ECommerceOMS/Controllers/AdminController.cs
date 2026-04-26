using ECommerceOMS.Data;
using ECommerceOMS.Models;
using ECommerceOMS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceOMS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var topProducts = await _context.OrderItems
                .Include(oi => oi.Product).ThenInclude(p => p!.Category)
                .GroupBy(oi => new { oi.ProductId, oi.Product!.Name, CategoryName = oi.Product.Category!.Name })
                .Select(g => new TopProductViewModel
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.Name,
                    CategoryName = g.Key.CategoryName,
                    TotalQuantitySold = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => x.Quantity * x.UnitPrice)
                })
                .OrderByDescending(x => x.TotalQuantitySold)
                .Take(5)
                .ToListAsync();

            var pendingOrders = await _context.Orders
                .Include(o => o.Customer)
                .Where(o => o.Status == OrderStatus.Pending || o.Status == OrderStatus.Processing)
                .OrderBy(o => o.OrderDate)
                .ToListAsync();

            var customerSummaries = await _context.Customers
                .Select(c => new CustomerOrderSummaryViewModel
                {
                    CustomerId = c.Id,
                    CustomerName = c.FirstName + " " + c.LastName,
                    Email = c.Email,
                    TotalOrders = c.Orders.Count,
                    TotalSpent = c.Orders.Sum(o => o.TotalAmount),
                    LastOrderDate = c.Orders.Max(o => (DateTime?)o.OrderDate)
                })
                .OrderByDescending(c => c.TotalSpent)
                .Take(10)
                .ToListAsync();

            var recentOrders = await _context.Orders
                .Include(o => o.Customer)
                .OrderByDescending(o => o.OrderDate)
                .Take(10)
                .ToListAsync();

            var vm = new AdminDashboardViewModel
            {
                TotalCustomers = await _context.Customers.CountAsync(),
                TotalProducts = await _context.Products.CountAsync(p => p.IsActive),
                TotalOrders = await _context.Orders.CountAsync(),
                TotalRevenue = await _context.Orders.SumAsync(o => o.TotalAmount),
                TopProducts = topProducts,
                PendingOrders = pendingOrders,
                CustomerSummaries = customerSummaries,
                RecentOrders = recentOrders
            };

            return View(vm);
        }

        // GET: Admin/Shipping
        public async Task<IActionResult> Shipping()
        {
            var vm = new ShippingManagementViewModel
            {
                PendingShipments = await _context.Orders
                    .Include(o => o.Customer)
                    .Where(o => o.Status == OrderStatus.Pending || o.Status == OrderStatus.Processing)
                    .OrderBy(o => o.OrderDate)
                    .ToListAsync(),

                ActiveShipments = await _context.ShippingDetails
                    .Include(sd => sd.Order).ThenInclude(o => o!.Customer)
                    .Where(sd => sd.Status == ShippingStatus.InTransit || sd.Status == ShippingStatus.OutForDelivery || sd.Status == ShippingStatus.Processing)
                    .OrderBy(sd => sd.EstimatedDelivery)
                    .ToListAsync(),

                DeliveredShipments = await _context.ShippingDetails
                    .Include(sd => sd.Order).ThenInclude(o => o!.Customer)
                    .Where(sd => sd.Status == ShippingStatus.Delivered)
                    .OrderByDescending(sd => sd.DeliveredOn)
                    .Take(20)
                    .ToListAsync()
            };

            return View(vm);
        }

        // GET: Admin/UpdateShipping/5
        public async Task<IActionResult> UpdateShipping(int orderId)
        {
            var order = await _context.Orders.Include(o => o.ShippingDetail).FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null) return NotFound();

            var sd = order.ShippingDetail;
            if (sd == null) return NotFound("No shipping detail found for this order.");

            var vm = new UpdateShippingViewModel
            {
                OrderId = order.Id,
                ShippingDetailId = sd.Id,
                OrderNumber = order.OrderNumber,
                Carrier = sd.Carrier,
                TrackingNumber = sd.TrackingNumber,
                EstimatedDelivery = sd.EstimatedDelivery,
                Status = sd.Status
            };
            return View(vm);
        }

        // POST: Admin/UpdateShipping
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateShipping(UpdateShippingViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var sd = await _context.ShippingDetails.FindAsync(vm.ShippingDetailId);
            if (sd == null) return NotFound();

            sd.Carrier = vm.Carrier;
            sd.TrackingNumber = vm.TrackingNumber;
            sd.EstimatedDelivery = vm.EstimatedDelivery;
            sd.Status = vm.Status;

            if (vm.Status == ShippingStatus.InTransit && sd.ShippedOn == null)
                sd.ShippedOn = DateTime.UtcNow;

            if (vm.Status == ShippingStatus.Delivered)
                sd.DeliveredOn = DateTime.UtcNow;

            // Sync order status
            var order = await _context.Orders.FindAsync(vm.OrderId);
            if (order != null)
            {
                order.Status = vm.Status switch
                {
                    ShippingStatus.Processing => OrderStatus.Processing,
                    ShippingStatus.InTransit => OrderStatus.Shipped,
                    ShippingStatus.OutForDelivery => OrderStatus.Shipped,
                    ShippingStatus.Delivered => OrderStatus.Delivered,
                    _ => order.Status
                };
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Shipping updated successfully.";
            return RedirectToAction(nameof(Shipping));
        }
    }
}
