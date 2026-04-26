// ViewModels/OrderSummaryViewModel.cs
using ECommerceApp.Models;

namespace ECommerceApp.ViewModels;

public class OrderSummaryViewModel
{
    public Order Order { get; set; } = new();
    public Customer Customer { get; set; } = new();
    public List<OrderDetail> OrderDetails { get; set; } = new();
}