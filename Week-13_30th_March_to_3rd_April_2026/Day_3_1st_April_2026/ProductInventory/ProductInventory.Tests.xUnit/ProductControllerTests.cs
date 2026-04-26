using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductInventory.API.Controllers;
using ProductInventory.API.Models;
using ProductInventory.API.Services;
using Xunit;

namespace ProductInventory.Tests.xUnit
{
    public class ProductControllerTests
    {
        private readonly Mock<IProductService> _mockService;
        private readonly ProductController _controller;

        public ProductControllerTests()
        {
            _mockService = new Mock<IProductService>();
            _controller  = new ProductController(_mockService.Object);
        }

        [Fact]
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
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, ok.StatusCode);
            var returned = Assert.IsType<Product>(ok.Value);
            Assert.Equal(expected.Id,            returned.Id);
            Assert.Equal(expected.Name,          returned.Name);
            Assert.Equal(expected.Price,         returned.Price);
            Assert.Equal(expected.StockQuantity, returned.StockQuantity);
            _mockService.Verify(s => s.GetProductByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetProduct_NonExistentId_Returns404NotFound()
        {
            // Arrange
            _mockService.Setup(s => s.GetProductByIdAsync(99)).ReturnsAsync((Product?)null);

            // Act
            var result = await _controller.GetProduct(99);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _mockService.Verify(s => s.GetProductByIdAsync(99), Times.Once);
        }

        [Theory]
        [InlineData(1, "Laptop",   999.99)]
        [InlineData(2, "Mouse",     29.99)]
        [InlineData(3, "Keyboard",  79.99)]
        public async Task GetProduct_MultipleExistingIds_ReturnsCorrectProduct(
            int id, string name, decimal price)
        {
            var product = new Product { Id = id, Name = name, Price = price };
            _mockService.Setup(s => s.GetProductByIdAsync(id)).ReturnsAsync(product);

            var result  = await _controller.GetProduct(id);

            var ok       = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsType<Product>(ok.Value);
            Assert.Equal(id,    returned.Id);
            Assert.Equal(name,  returned.Name);
            Assert.Equal(price, returned.Price);
        }

        [Theory]
        [InlineData(99)]
        [InlineData(100)]
        [InlineData(0)]
        public async Task GetProduct_NonExistentIds_Return404(int id)
        {
            _mockService.Setup(s => s.GetProductByIdAsync(id)).ReturnsAsync((Product?)null);
            var result = await _controller.GetProduct(id);
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
