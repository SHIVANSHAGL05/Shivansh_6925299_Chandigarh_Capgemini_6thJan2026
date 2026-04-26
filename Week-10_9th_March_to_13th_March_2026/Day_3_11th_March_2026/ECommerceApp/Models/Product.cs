// Models/Product.cs
namespace ECommerceApp.Models;

public class Product
{
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }

    // FK → Category
    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    // Navigation — one Product can appear in many OrderDetails
    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}