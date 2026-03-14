// Models/Order.cs
namespace ECommerceApp.Models;

public class Order
{
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }

    // FK → Customer
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }

    // Navigation — one Order has many OrderDetails
    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}