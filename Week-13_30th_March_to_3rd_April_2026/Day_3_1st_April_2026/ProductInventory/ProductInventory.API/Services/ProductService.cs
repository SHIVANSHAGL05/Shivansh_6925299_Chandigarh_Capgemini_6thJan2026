using ProductInventory.API.Models;

namespace ProductInventory.API.Services
{
    public class ProductService : IProductService
    {
        private static readonly List<Product> _products = new()
        {
            new Product { Id = 1, Name = "Laptop",   Description = "High-performance laptop",  Price = 999.99m, StockQuantity = 50  },
            new Product { Id = 2, Name = "Mouse",    Description = "Wireless ergonomic mouse", Price = 29.99m,  StockQuantity = 200 },
            new Product { Id = 3, Name = "Keyboard", Description = "Mechanical keyboard",      Price = 79.99m,  StockQuantity = 150 },
        };

        public Task<Product?> GetProductByIdAsync(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            return Task.FromResult(product);
        }
    }
}
