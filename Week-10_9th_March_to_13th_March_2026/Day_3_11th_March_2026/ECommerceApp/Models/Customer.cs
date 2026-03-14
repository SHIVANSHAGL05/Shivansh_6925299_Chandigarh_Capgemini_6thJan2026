// Models/Customer.cs
namespace ECommerceApp.Models;

public class Customer
{
    public int CustomerId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;

    // Navigation — one Customer can place many Orders
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}