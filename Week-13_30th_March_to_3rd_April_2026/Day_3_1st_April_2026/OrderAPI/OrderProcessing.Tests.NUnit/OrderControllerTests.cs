using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using OrderProcessing.API.Controllers;
using OrderProcessing.API.Models;
using OrderProcessing.API.Services;

namespace OrderProcessing.Tests.NUnit
{
    /// <summary>
    /// NUnit tests for OrderController — mirrors the xUnit suite.
    ///
    /// NUnit pattern: single shared instance → [SetUp] runs before each test.
    /// </summary>
    [TestFixture]
    public class OrderControllerTests
    {
        // ── Shared setup ────────────────────────────────────────────────────────
        private Mock<IOrderService> _mockService = null!;
        private OrderController     _controller  = null!;

        [SetUp]
        public void SetUp()
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
            CustomerId  = 0,
            ProductName = "",
            Quantity    = 0,
            TotalAmount = 0
        };

        // ── Test 1: Valid order → 201 Created ────────────────────────────────────
        [Test]
        public async Task PlaceOrder_ValidOrder_Returns201Created()
        {
            // Arrange
            var order = ValidOrder();
            _mockService.Setup(s => s.PlaceOrderAsync(order)).ReturnsAsync(true);

            // Act
            var result = await _controller.PlaceOrder(order);

            // Assert
            Assert.That(result, Is.InstanceOf<StatusCodeResult>());
            var statusResult = (StatusCodeResult)result;
            Assert.That(statusResult.StatusCode, Is.EqualTo(201));

            _mockService.Verify(s => s.PlaceOrderAsync(order), Times.Once);
        }

        // ── Test 2: Invalid order → 400 Bad Request ──────────────────────────────
        [Test]
        public async Task PlaceOrder_InvalidOrder_Returns400BadRequest()
        {
            // Arrange
            var order = InvalidOrder();
            _mockService.Setup(s => s.PlaceOrderAsync(order)).ReturnsAsync(false);

            // Act
            var result = await _controller.PlaceOrder(order);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestResult>());

            _mockService.Verify(s => s.PlaceOrderAsync(order), Times.Once);
        }

        // ── Test 3: Parameterised – service returns true → always 201 ────────────
        [TestCase(1, "Laptop",   2, 1999.98)]
        [TestCase(2, "Mouse",    5,  149.95)]
        [TestCase(3, "Monitor",  1,  399.99)]
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

            Assert.That(result, Is.InstanceOf<StatusCodeResult>());
            Assert.That(((StatusCodeResult)result).StatusCode, Is.EqualTo(201));
        }

        // ── Test 4: Parameterised – service returns false → always 400 ───────────
        [TestCase(0,  "",        0, 0)]
        [TestCase(-1, "Widget", -1, 0)]
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

            Assert.That(result, Is.InstanceOf<BadRequestResult>());
        }
    }
}
