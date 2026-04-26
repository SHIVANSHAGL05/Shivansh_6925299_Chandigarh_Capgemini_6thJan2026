using Microsoft.AspNetCore.Mvc;
using Moq;
using OrderProcessing.API.Controllers;
using OrderProcessing.API.Models;
using OrderProcessing.API.Services;
using Xunit;

namespace OrderProcessing.Tests.xUnit
{
    /// <summary>
    /// xUnit tests for OrderController.
    /// Uses Moq to isolate the controller from IOrderService.
    ///
    /// xUnit pattern: new instance per test → constructor = setup.
    /// </summary>
    public class OrderControllerTests
    {
        // ── Shared setup ────────────────────────────────────────────────────────
        private readonly Mock<IOrderService> _mockService;
        private readonly OrderController     _controller;

        public OrderControllerTests()
        {
            _mockService = new Mock<IOrderService>();
            _controller  = new OrderController(_mockService.Object);
        }

        // ── Helpers ─────────────────────────────────────────────────────────────
        private static Order ValidOrder() => new()
        {
            CustomerId  = 1,
            ProductName = "Laptop",
            Quantity    = 2,
            TotalAmount = 1999.98m
        };

        private static Order InvalidOrder() => new()
        {
            CustomerId  = 0,        // invalid
            ProductName = "",       // invalid
            Quantity    = 0,        // invalid
            TotalAmount = 0         // invalid
        };

        // ── Test 1: Valid order → 201 Created ────────────────────────────────────
        [Fact]
        public async Task PlaceOrder_ValidOrder_Returns201Created()
        {
            // Arrange – service returns true (success)
            var order = ValidOrder();
            _mockService.Setup(s => s.PlaceOrderAsync(order)).ReturnsAsync(true);

            // Act
            var result = await _controller.PlaceOrder(order);

            // Assert
            var statusResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(201, statusResult.StatusCode);

            // Verify service was called exactly once
            _mockService.Verify(s => s.PlaceOrderAsync(order), Times.Once);
        }

        // ── Test 2: Invalid order → 400 Bad Request ──────────────────────────────
        [Fact]
        public async Task PlaceOrder_InvalidOrder_Returns400BadRequest()
        {
            // Arrange – service returns false (failure)
            var order = InvalidOrder();
            _mockService.Setup(s => s.PlaceOrderAsync(order)).ReturnsAsync(false);

            // Act
            var result = await _controller.PlaceOrder(order);

            // Assert
            Assert.IsType<BadRequestResult>(result);

            // Verify service was called exactly once
            _mockService.Verify(s => s.PlaceOrderAsync(order), Times.Once);
        }

        // ── Test 3: Parameterised – service returns true → always 201 ────────────
        [Theory]
        [InlineData(1, "Laptop",   2, 1999.98)]
        [InlineData(2, "Mouse",    5,  149.95)]
        [InlineData(3, "Monitor",  1,  399.99)]
        public async Task PlaceOrder_WhenServiceSucceeds_Returns201(
            int customerId, string product, int qty, decimal total)
        {
            var order = new Order
            {
                CustomerId  = customerId,
                ProductName = product,
                Quantity    = qty,
                TotalAmount = total
            };
            _mockService.Setup(s => s.PlaceOrderAsync(order)).ReturnsAsync(true);

            var result = await _controller.PlaceOrder(order);

            var statusResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(201, statusResult.StatusCode);
        }

        // ── Test 4: Parameterised – service returns false → always 400 ───────────
        [Theory]
        [InlineData(0,  "",        0, 0)]
        [InlineData(-1, "Widget", -1, 0)]
        public async Task PlaceOrder_WhenServiceFails_Returns400(
            int customerId, string product, int qty, decimal total)
        {
            var order = new Order
            {
                CustomerId  = customerId,
                ProductName = product,
                Quantity    = qty,
                TotalAmount = total
            };
            _mockService.Setup(s => s.PlaceOrderAsync(order)).ReturnsAsync(false);

            var result = await _controller.PlaceOrder(order);

            Assert.IsType<BadRequestResult>(result);
        }
    }
}
