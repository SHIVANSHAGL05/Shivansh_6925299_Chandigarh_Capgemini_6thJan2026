using ECommerceOrderAPI.Data;
using ECommerceOrderAPI.Models;
using log4net;

namespace ECommerceOrderAPI.Services
{
    public class PaymentService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PaymentService));
        private readonly AppDbContext _context;

        public PaymentService(AppDbContext context)
        {
            _context = context;
        }

        public Payment ProcessPayment(int orderId, decimal amount)
        {
            log.Info($"Payment request received | OrderId: {orderId} | Amount: {amount}");

            var order = _context.Orders.FirstOrDefault(o => o.Id == orderId);

            if (order == null)
            {
                log.Error($"Payment failed - Order {orderId} not found");
                throw new Exception($"Order {orderId} not found.");
            }

            if (amount <= 0)
            {
                log.Warn($"Invalid payment amount {amount} for OrderId: {orderId}");
                throw new Exception("Payment amount must be greater than zero.");
            }

            try
            {
                var startTime = DateTime.UtcNow;

                SimulateExternalPaymentGateway(orderId);

                var elapsed = (DateTime.UtcNow - startTime).TotalSeconds;

                if (elapsed > 5)
                    log.Warn($"Payment delay detected for OrderId: {orderId} | Took {elapsed:F1} seconds");

                var payment = new Payment
                {
                    OrderId = orderId,
                    Amount = amount,
                    Status = "Success",
                    ProcessedAt = DateTime.UtcNow
                };

                order.Status = "Completed";
                _context.Payments.Add(payment);
                _context.SaveChanges();

                log.Info($"Payment successful for OrderId: {orderId} | Amount: {amount}");
                return payment;
            }
            catch (TimeoutException ex)
            {
                log.Error($"Payment failed: Timeout for OrderId: {orderId}", ex);
                throw;
            }
            catch (Exception ex)
            {
                log.Error($"Payment failed for OrderId: {orderId} | Reason: {ex.Message}", ex);
                throw;
            }
        }

        private void SimulateExternalPaymentGateway(int orderId)
        {
            if (orderId == 999)
                throw new TimeoutException("External payment gateway timed out.");
        }
    }
}
