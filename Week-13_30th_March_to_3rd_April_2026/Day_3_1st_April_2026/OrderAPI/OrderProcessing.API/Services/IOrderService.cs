using OrderProcessing.API.Models;

namespace OrderProcessing.API.Services
{
    public interface IOrderService
    {
        /// <summary>
        /// Places an order.
        /// Returns true if successful, false otherwise.
        /// </summary>
        Task<bool> PlaceOrderAsync(Order order);
    }
}
