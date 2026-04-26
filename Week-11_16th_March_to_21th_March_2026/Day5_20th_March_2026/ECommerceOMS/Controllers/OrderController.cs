using ECommerceOMS.Data;
using ECommerceOMS.Models;
using ECommerceOMS.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ECommerceOMS.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Order
        public async Task<IActionResult> Index(string? status, string? search)
        {
            var query = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.ShippingDetail)
                .Include(o => o.OrderItems)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<OrderStatus>(status, out var parsed))
                query = query.Where(o => o.Status == parsed);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(o => o.OrderNumber.Contains(search) ||
                                         o.Customer!.FirstName.Contains(search) ||
                                         o.Customer!.LastName.Contains(search));

            var orders = await query.OrderByDescending(o => o.OrderDate).ToListAsync();
            ViewBag.StatusList = Enum.GetValues<OrderStatus>().Select(s => new SelectListItem(s.ToString(), s.ToString()));
            ViewBag.SelectedStatus = status;
            ViewBag.Search = search;
            return View(orders);
        }

        // GET: Order/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product).ThenInclude(p => p!.Category)
                .Include(o => o.ShippingDetail)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            var vm = new OrderDetailViewModel
            {
                Order = order,
                OrderItems = order.OrderItems.ToList(),
                ShippingDetail = order.ShippingDetail
            };
            return View(vm);
        }

        // GET: Order/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Customers = new SelectList(await _context.Customers.Where(c => c.IsActive).ToListAsync(), "Id", "FullName");
            ViewBag.Products = await _context.Products.Include(p => p.Category).Where(p => p.IsActive).ToListAsync();
            return View(new PlaceOrderViewModel());
        }

        // POST: Order/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int customerId, string recipientName, string shippingAddress,
            string city, string state, string postalCode, string country,
            string? notes, List<int> productIds, List<int> quantities)
        {
            if (productIds == null || !productIds.Any())
            {
                TempData["Error"] = "Please add at least one product to the order.";
                return RedirectToAction(nameof(Create));
            }

            var orderNumber = "ORD-" + DateTime.UtcNow.ToString("yyyyMMdd") + "-" + new Random().Next(1000, 9999);

            var order = new Order
            {
                CustomerId = customerId,
                OrderNumber = orderNumber,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                Notes = notes
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            decimal total = 0;
            for (int i = 0; i < productIds.Count; i++)
            {
                var product = await _context.Products.FindAsync(productIds[i]);
                if (product == null) continue;

                var qty = quantities[i];
                var item = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = product.Id,
                    Quantity = qty,
                    UnitPrice = product.Price
                };
                _context.OrderItems.Add(item);
                total += product.Price * qty;

                // Reduce stock
                product.StockQuantity -= qty;
                _context.Update(product);
            }

            order.TotalAmount = total;

            var shipping = new ShippingDetail
            {
                OrderId = order.Id,
                RecipientName = recipientName,
                ShippingAddress = shippingAddress,
                City = city,
                State = state,
                PostalCode = postalCode,
                Country = country,
                Status = ShippingStatus.NotShipped
            };
            _context.ShippingDetails.Add(shipping);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Order {orderNumber} placed successfully!";
            return RedirectToAction(nameof(Details), new { id = order.Id });
        }

        // GET: Order/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var order = await _context.Orders.Include(o => o.Customer).FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) return NotFound();
            ViewBag.StatusList = new SelectList(Enum.GetValues<OrderStatus>().Select(s => new { Value = s, Text = s.ToString() }), "Value", "Text", order.Status);
            return View(order);
        }

        // POST: Order/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OrderStatus status, string? notes)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();
            order.Status = status;
            order.Notes = notes;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Order updated.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: Order/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();
            if (order.Status == OrderStatus.Shipped || order.Status == OrderStatus.Delivered)
            {
                TempData["Error"] = "Cannot cancel a shipped or delivered order.";
                return RedirectToAction(nameof(Details), new { id });
            }

            order.Status = OrderStatus.Cancelled;

            // Restore stock
            foreach (var item in order.OrderItems)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    product.StockQuantity += item.Quantity;
                    _context.Update(product);
                }
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Order cancelled and stock restored.";
            return RedirectToAction(nameof(Index));
        }
    }
}
