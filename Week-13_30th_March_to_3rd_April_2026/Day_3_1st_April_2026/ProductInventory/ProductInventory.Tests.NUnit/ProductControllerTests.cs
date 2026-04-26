using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ProductInventory.API.Controllers;
using ProductInventory.API.Models;
using ProductInventory.API.Services;

namespace ProductInventory.Tests.NUnit
{
    [TestFixture]
    public class ProductControllerTests
    {
        private Mock<IProductService> _mockService = null!;
        private ProductController     _controller  = null!;

        [SetUp]
        public void SetUp()
        {
            _mockService = new Mock<IProductService>();
            _controller  = new ProductController(_mockService.Object);
        }

        [Test]
        public async Task GetProduct_ExistingId_Returns200OkWithProduct()
        {
            // Arrange
            var expected = new Product
            {
                Id = 1, Name = "Laptop", Description = "High-performance laptop",
                Price = 999.99m, StockQuantity = 50
            };
            _mockService.Setup(s => s.GetProductByIdAsync(1)).ReturnsAsync(expected);

            // Act
            var result = await _controller.GetProduct(1);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var ok = (OkObjectResult)result;
            Assert.That(ok.StatusCode, Is.EqualTo(200));
            var returned = ok.Value as Product;
            Assert.That(returned,               Is.Not.Null);
            Assert.That(returned!.Id,           Is.EqualTo(expected.Id));
            Assert.That(returned.Name,          Is.EqualTo(expected.Name));
            Assert.That(returned.Price,         Is.EqualTo(expected.Price));
            Assert.That(returned.StockQuantity, Is.EqualTo(expected.StockQuantity));
            _mockService.Verify(s => s.GetProductByIdAsync(1), Times.Once);
        }

        [Test]
        public async Task GetProduct_NonExistentId_Returns404NotFound()
        {
            // Arrange
            _mockService.Setup(s => s.GetProductByIdAsync(99)).ReturnsAsync((Product?)null);

            // Act
            var result = await _controller.GetProduct(99);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
            _mockService.Verify(s => s.GetProductByIdAsync(99), Times.Once);
        }

        [TestCase(1, "Laptop",   999.99)]
        [TestCase(2, "Mouse",     29.99)]
        [TestCase(3, "Keyboard",  79.99)]
        public async Task GetProduct_MultipleExistingIds_ReturnsCorrectProduct(
            int id, string name, decimal price)
        {
            var product = new Product { Id = id, Name = name, Price = price };
            _mockService.Setup(s => s.GetProductByIdAsync(id)).ReturnsAsync(product);

            var result = await _controller.GetProduct(id);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var returned = ((OkObjectResult)result).Value as Product;
            Assert.That(returned,       Is.Not.Null);
            Assert.That(returned!.Id,   Is.EqualTo(id));
            Assert.That(returned.Name,  Is.EqualTo(name));
            Assert.That(returned.Price, Is.EqualTo(price));
        }

        [TestCase(99)]
        [TestCase(100)]
        [TestCase(0)]
        public async Task GetProduct_NonExistentIds_Return404(int id)
        {
            _mockService.Setup(s => s.GetProductByIdAsync(id)).ReturnsAsync((Product?)null);
            var result = await _controller.GetProduct(id);
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }
    }
}
