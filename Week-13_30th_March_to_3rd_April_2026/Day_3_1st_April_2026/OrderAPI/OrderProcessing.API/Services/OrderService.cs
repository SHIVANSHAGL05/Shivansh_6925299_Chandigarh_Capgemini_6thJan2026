using OrderProcessing.API.Models;

namespace OrderProcessing.API.Services
{
    /// <summary>
    /// Concrete implementation of IOrderService.
    /// Validates and persists the order (in-memory for demo purposes).
    /// </summary>
    public class OrderService : IOrderService
    {
        private static readonly List<Order> _orders = new();

        public Task<bool> PlaceOrderAsync(Order order)
        {
            // Basic validation — a real app would also hit a database
            if (order == null
                || order.CustomerId <= 0
                || string.IsNullOrWhiteSpace(order.ProductName)
                || order.Quantity <= 0
                || order.TotalAmount <= 0)
            {
                return Task.FromResult(false);
            }

            order.Id = _orders.Count + 1;
            _orders.Add(order);
            return Task.FromResult(true);
        }
    }
}
