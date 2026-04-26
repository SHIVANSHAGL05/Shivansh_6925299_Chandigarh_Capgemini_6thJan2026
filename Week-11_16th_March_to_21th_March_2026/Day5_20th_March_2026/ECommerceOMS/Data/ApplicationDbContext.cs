using ECommerceOMS.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECommerceOMS.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<ShippingDetail> ShippingDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── Customer → Orders (One-to-Many) ──────────────────────────
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // ── Order → OrderItems (One-to-Many) ─────────────────────────
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // ── Product → OrderItems (Many-to-Many via OrderItem) ─────────
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── Category → Products (One-to-Many) ────────────────────────
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── Order → ShippingDetail (One-to-One) ───────────────────────
            modelBuilder.Entity<Order>()
                .HasOne(o => o.ShippingDetail)
                .WithOne(sd => sd.Order)
                .HasForeignKey<ShippingDetail>(sd => sd.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // ── Decimal precision ─────────────────────────────────────────
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.UnitPrice)
                .HasColumnType("decimal(18,2)");

            // ── Ignore computed property ──────────────────────────────────
            modelBuilder.Entity<OrderItem>()
                .Ignore(oi => oi.Subtotal);

            // ── Seed Data ─────────────────────────────────────────────────
            SeedData(modelBuilder);
        }

        private static void SeedData(ModelBuilder modelBuilder)
        {
            // Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Electronics", Description = "Electronic gadgets and devices", DisplayOrder = 1 },
                new Category { Id = 2, Name = "Clothing", Description = "Apparel and accessories", DisplayOrder = 2 },
                new Category { Id = 3, Name = "Books", Description = "Books, eBooks and magazines", DisplayOrder = 3 },
                new Category { Id = 4, Name = "Home & Kitchen", Description = "Home appliances and kitchenware", DisplayOrder = 4 },
                new Category { Id = 5, Name = "Sports", Description = "Sports and outdoor equipment", DisplayOrder = 5 }
            );

            // Products
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Wireless Bluetooth Headphones", Description = "Premium noise-cancelling wireless headphones with 30hr battery.", Price = 2999.00m, StockQuantity = 50, CategoryId = 1, SKU = "ELEC-001" },
                new Product { Id = 2, Name = "Mechanical Keyboard", Description = "RGB backlit mechanical keyboard, compact TKL layout.", Price = 3499.00m, StockQuantity = 30, CategoryId = 1, SKU = "ELEC-002" },
                new Product { Id = 3, Name = "USB-C Hub 7-in-1", Description = "Multi-port USB-C hub with HDMI, USB 3.0, SD card reader.", Price = 1299.00m, StockQuantity = 75, CategoryId = 1, SKU = "ELEC-003" },
                new Product { Id = 4, Name = "Men's Casual T-Shirt", Description = "100% cotton comfortable daily wear t-shirt.", Price = 499.00m, StockQuantity = 200, CategoryId = 2, SKU = "CLO-001" },
                new Product { Id = 5, Name = "Women's Running Shoes", Description = "Lightweight breathable running shoes with memory foam sole.", Price = 2199.00m, StockQuantity = 60, CategoryId = 2, SKU = "CLO-002" },
                new Product { Id = 6, Name = "Clean Code by Robert Martin", Description = "A handbook of agile software craftsmanship.", Price = 799.00m, StockQuantity = 100, CategoryId = 3, SKU = "BOK-001" },
                new Product { Id = 7, Name = "The Pragmatic Programmer", Description = "Your journey to mastery, 20th Anniversary Edition.", Price = 899.00m, StockQuantity = 80, CategoryId = 3, SKU = "BOK-002" },
                new Product { Id = 8, Name = "Non-stick Cookware Set", Description = "5-piece granite non-stick cookware set.", Price = 1899.00m, StockQuantity = 40, CategoryId = 4, SKU = "HMK-001" },
                new Product { Id = 9, Name = "Yoga Mat Premium", Description = "6mm thick anti-slip yoga mat with carrying strap.", Price = 699.00m, StockQuantity = 120, CategoryId = 5, SKU = "SPT-001" },
                new Product { Id = 10, Name = "Resistance Bands Set", Description = "Set of 5 resistance bands with door anchor and handles.", Price = 549.00m, StockQuantity = 90, CategoryId = 5, SKU = "SPT-002" }
            );

            // Customers
            modelBuilder.Entity<Customer>().HasData(
                new Customer { Id = 1, FirstName = "Aarav", LastName = "Sharma", Email = "aarav.sharma@example.com", Phone = "9876543210", Address = "12 MG Road", City = "Bengaluru", PostalCode = "560001" },
                new Customer { Id = 2, FirstName = "Priya", LastName = "Patel", Email = "priya.patel@example.com", Phone = "9812345678", Address = "45 Nehru Street", City = "Mumbai", PostalCode = "400001" },
                new Customer { Id = 3, FirstName = "Rohan", LastName = "Verma", Email = "rohan.verma@example.com", Phone = "9934567890", Address = "7 Connaught Place", City = "New Delhi", PostalCode = "110001" }
            );
        }
    }
}
