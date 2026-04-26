using Microsoft.AspNetCore.Mvc;
using OrderProcessing.API.Models;
using OrderProcessing.API.Services;

namespace OrderProcessing.API.Controllers
{
    /// <summary>
    /// Handles order placement requests.
    /// Depends on IOrderService injected via constructor DI.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // POST api/order
        // Returns 201 Created on success, 400 Bad Request on failure.
        [HttpPost]
        public async Task<IActionResult> PlaceOrder([FromBody] Order order)
        {
            var success = await _orderService.PlaceOrderAsync(order);

            if (!success)
                return BadRequest();        // 400

            return StatusCode(201);         // 201 Created
        }
    }
}
