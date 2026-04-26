using ECommerceOrderAPI.DTOs;
using ECommerceOrderAPI.Services;
using log4net;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceOrderAPI.Controllers
{
    [ApiController]
    [Route("api/payment")]
    public class PaymentController : ControllerBase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PaymentController));
        private readonly PaymentService _paymentService;

        public PaymentController(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("process")]
        public IActionResult ProcessPayment([FromBody] PaymentRequest request)
        {
            if (request.OrderId <= 0 || request.Amount <= 0)
            {
                log.Warn($"Invalid payment request | OrderId: {request.OrderId} | Amount: {request.Amount}");
                return BadRequest(new { message = "Invalid payment data." });
            }

            try
            {
                var payment = _paymentService.ProcessPayment(request.OrderId, request.Amount);
                return Ok(new { message = "Payment processed successfully.", payment });
            }
            catch (TimeoutException ex)
            {
                log.Error($"Payment timeout for OrderId: {request.OrderId}", ex);
                return StatusCode(504, new { message = "Payment gateway timed out. Please try again." });
            }
            catch (Exception ex)
            {
                log.Error($"Payment processing failed for OrderId: {request.OrderId}", ex);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
