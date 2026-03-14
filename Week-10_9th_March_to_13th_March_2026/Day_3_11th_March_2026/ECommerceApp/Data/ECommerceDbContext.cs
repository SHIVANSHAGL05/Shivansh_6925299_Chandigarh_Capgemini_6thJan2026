// Data/ECommerceDbContext.cs
using Microsoft.EntityFrameworkCore;
using ECommerceApp.Models;

namespace ECommerceApp.Data;

public class ECommerceDbContext : DbContext
{
    public ECommerceDbContext(DbContextOptions<ECommerceDbContext> options)
        : base(options) { }

    // DbSets — each becomes a table
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ── Category → Products (One to Many) ───────────────────────────
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // ── Customer → Orders (One to Many) ─────────────────────────────
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Customer)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        // ── Order → OrderDetails (One to Many) ──────────────────────────
        modelBuilder.Entity<OrderDetail>()
            .HasOne(od => od.Order)
            .WithMany(o => o.OrderDetails)
            .HasForeignKey(od => od.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // ── Product → OrderDetails (One to Many) ────────────────────────
        modelBuilder.Entity<OrderDetail>()
            .HasOne(od => od.Product)
            .WithMany(p => p.OrderDetails)
            .HasForeignKey(od => od.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // ── Decimal precision ────────────────────────────────────────────
        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Order>()
            .Property(o => o.TotalAmount)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<OrderDetail>()
            .Property(od => od.UnitPrice)
            .HasColumnType("decimal(18,2)");

        // ── Seed Data ────────────────────────────────────────────────────
        modelBuilder.Entity<Category>().HasData(
            new Category { CategoryId = 1, Name = "Electronics", Description = "Electronic gadgets and devices" },
            new Category { CategoryId = 2, Name = "Books", Description = "Academic and fiction books" },
            new Category { CategoryId = 3, Name = "Clothing", Description = "Men and women clothing" }
        );

        modelBuilder.Entity<Product>().HasData(
            new Product { ProductId = 1, Name = "Laptop", Price = 75000, StockQuantity = 10, CategoryId = 1 },
            new Product { ProductId = 2, Name = "Smartphone", Price = 25000, StockQuantity = 25, CategoryId = 1 },
            new Product { ProductId = 3, Name = "Clean Code", Price = 599, StockQuantity = 50, CategoryId = 2 },
            new Product { ProductId = 4, Name = "T-Shirt", Price = 299, StockQuantity = 100, CategoryId = 3 }
        );

        modelBuilder.Entity<Customer>().HasData(
            new Customer { CustomerId = 1, FullName = "Harsh Tanwar", Email = "harsh@gmail.com", Address = "Delhi, India" },
            new Customer { CustomerId = 2, FullName = "Priya Sharma", Email = "priya@gmail.com", Address = "Mumbai, India" }
        );

        modelBuilder.Entity<Order>().HasData(
            new Order { OrderId = 1, CustomerId = 1, OrderDate = new DateTime(2026, 3, 1), TotalAmount = 75599 },
            new Order { OrderId = 2, CustomerId = 2, OrderDate = new DateTime(2026, 3, 5), TotalAmount = 25299 }
        );

        modelBuilder.Entity<OrderDetail>().HasData(
            new OrderDetail { OrderDetailId = 1, OrderId = 1, ProductId = 1, Quantity = 1, UnitPrice = 75000 },
            new OrderDetail { OrderDetailId = 2, OrderId = 1, ProductId = 3, Quantity = 1, UnitPrice = 599 },
            new OrderDetail { OrderDetailId = 3, OrderId = 2, ProductId = 2, Quantity = 1, UnitPrice = 25000 },
            new OrderDetail { OrderDetailId = 4, OrderId = 2, ProductId = 4, Quantity = 1, UnitPrice = 299 }
        );
    }
}