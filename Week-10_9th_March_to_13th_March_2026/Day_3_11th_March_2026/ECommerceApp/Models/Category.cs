// Models/Category.cs
namespace ECommerceApp.Models;

public class Category
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // Navigation — one Category has many Products
    public ICollection<Product> Products { get; set; } = new List<Product>();
}