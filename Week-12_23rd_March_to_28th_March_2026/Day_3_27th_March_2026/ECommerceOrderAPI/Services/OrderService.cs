using ECommerceOrderAPI.Data;
using ECommerceOrderAPI.Models;
using log4net;

namespace ECommerceOrderAPI.Services
{
    public class OrderService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(OrderService));
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        public Order PlaceOrder(int userId, int productId, int quantity)
        {
            log.Info($"Order started for user {userId} | Product: {productId} | Quantity: {quantity}");

            var product = _context.Products.FirstOrDefault(p => p.Id == productId);

            if (product == null)
            {
                log.Error($"Order failed - Product {productId} not found for user {userId}");
                throw new Exception($"Product with ID {productId} not found.");
            }

            if (product.Stock < quantity)
            {
                log.Error($"Order failed - Insufficient stock for Product {productId}. Requested: {quantity}, Available: {product.Stock}");
                throw new Exception("Insufficient stock.");
            }

            try
            {
                var existingOrder = _context.Orders
                    .FirstOrDefault(o => o.UserId == userId && o.ProductId == productId && o.Status == "Pending");

                if (existingOrder != null)
                {
                    log.Warn($"Duplicate order detected for user {userId} on product {productId}");
                    throw new Exception("Duplicate order detected. Please check your existing orders.");
                }

                var order = new Order
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = quantity,
                    TotalAmount = product.Price * quantity,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow
                };

                product.Stock -= quantity;
                _context.Orders.Add(order);
                _context.SaveChanges();

                log.Info($"Order placed successfully for user {userId} | OrderId: {order.Id} | Total: {order.TotalAmount}");
                return order;
            }
            catch (Exception ex) when (!(ex.Message.Contains("Duplicate") || ex.Message.Contains("stock")))
            {
                log.Error($"Order placement failed for user {userId}", ex);
                throw;
            }
        }
    }
}
