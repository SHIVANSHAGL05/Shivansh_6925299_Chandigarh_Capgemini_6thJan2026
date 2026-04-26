using Microsoft.EntityFrameworkCore;
using ECommerceOrderAPI.Models;

namespace ECommerceOrderAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Name = "Shivansh Sharma", Email = "shivansh@gmail.com", Password = "admin123" },
                new User { Id = 2, Name = "Shiva Verma", Email = "shiva@gmail.com", Password = "user123" }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Wireless Headphones", Price = 2999.00m, Stock = 50 },
                new Product { Id = 2, Name = "Mechanical Keyboard", Price = 4599.00m, Stock = 30 }
            );

            modelBuilder.Entity<Order>().HasData(
                new Order { Id = 1, UserId = 1, ProductId = 1, Quantity = 1, TotalAmount = 2999.00m, Status = "Completed", CreatedAt = DateTime.UtcNow },
                new Order { Id = 2, UserId = 2, ProductId = 2, Quantity = 2, TotalAmount = 9198.00m, Status = "Pending", CreatedAt = DateTime.UtcNow }
            );
        }
    }
}
