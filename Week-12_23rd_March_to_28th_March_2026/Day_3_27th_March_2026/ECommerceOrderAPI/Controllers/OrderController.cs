using ECommerceOrderAPI.DTOs;
using ECommerceOrderAPI.Services;
using log4net;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceOrderAPI.Controllers
{
    [ApiController]
    [Route("api/order")]
    public class OrderController : ControllerBase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(OrderController));
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("place")]
        public IActionResult PlaceOrder([FromBody] PlaceOrderRequest request)
        {
            if (request.UserId <= 0 || request.ProductId <= 0 || request.Quantity <= 0)
            {
                log.Warn($"Invalid order request data | UserId: {request.UserId} | ProductId: {request.ProductId} | Quantity: {request.Quantity}");
                return BadRequest(new { message = "Invalid order data. All fields must be greater than zero." });
            }

            try
            {
                var order = _orderService.PlaceOrder(request.UserId, request.ProductId, request.Quantity);
                return Ok(new { message = "Order placed successfully.", order });
            }
            catch (Exception ex)
            {
                log.Error($"Order placement failed for UserId: {request.UserId}", ex);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
