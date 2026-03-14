// Models/OrderDetail.cs
namespace ECommerceApp.Models;

public class OrderDetail
{
    public int OrderDetailId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    // FK → Order
    public int OrderId { get; set; }
    public Order? Order { get; set; }

    // FK → Product
    public int ProductId { get; set; }
    public Product? Product { get; set; }
}