using ECommerceOMS.Models;
using System.ComponentModel.DataAnnotations;

namespace ECommerceOMS.ViewModels
{
    // ── Order Detail (Order + Items + Shipping) ───────────────────────────────
    public class OrderDetailViewModel
    {
        public Order Order { get; set; } = null!;
        public List<OrderItem> OrderItems { get; set; } = new();
        public ShippingDetail? ShippingDetail { get; set; }
        public decimal TotalAmount => OrderItems.Sum(i => i.Quantity * i.UnitPrice);
    }

    // ── Place Order ───────────────────────────────────────────────────────────
    public class PlaceOrderViewModel
    {
        [Required]
        [Display(Name = "Customer")]
        public int CustomerId { get; set; }

        public List<CartItemViewModel> CartItems { get; set; } = new();

        [StringLength(500)]
        public string? Notes { get; set; }

        // Shipping
        [Required]
        [StringLength(100)]
        [Display(Name = "Recipient Name")]
        public string RecipientName { get; set; } = string.Empty;

        [Required]
        [StringLength(250)]
        [Display(Name = "Shipping Address")]
        public string ShippingAddress { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string State { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; } = string.Empty;

        [Required]
        [StringLength(60)]
        public string Country { get; set; } = "India";
    }

    public class CartItemViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal => Quantity * UnitPrice;
    }

    // ── Admin Dashboard ───────────────────────────────────────────────────────
    public class AdminDashboardViewModel
    {
        public int TotalCustomers { get; set; }
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }

        public List<TopProductViewModel> TopProducts { get; set; } = new();
        public List<Order> PendingOrders { get; set; } = new();
        public List<CustomerOrderSummaryViewModel> CustomerSummaries { get; set; } = new();
        public List<Order> RecentOrders { get; set; } = new();
    }

    public class TopProductViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public int TotalQuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class CustomerOrderSummaryViewModel
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
        public DateTime? LastOrderDate { get; set; }
    }

    // ── Product Listing with Filter ───────────────────────────────────────────
    public class ProductListViewModel
    {
        public List<Product> Products { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
        public int? SelectedCategoryId { get; set; }
        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; }
    }

    // ── Customer History ──────────────────────────────────────────────────────
    public class CustomerHistoryViewModel
    {
        public Customer Customer { get; set; } = null!;
        public List<OrderDetailViewModel> Orders { get; set; } = new();
        public decimal TotalSpent => Orders.Sum(o => o.TotalAmount);
        public int TotalOrders => Orders.Count;
    }

    // ── Shipping Management ───────────────────────────────────────────────────
    public class ShippingManagementViewModel
    {
        public List<Order> PendingShipments { get; set; } = new();
        public List<ShippingDetail> ActiveShipments { get; set; } = new();
        public List<ShippingDetail> DeliveredShipments { get; set; } = new();
    }

    // ── Update Shipping ───────────────────────────────────────────────────────
    public class UpdateShippingViewModel
    {
        public int OrderId { get; set; }
        public int ShippingDetailId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Carrier { get; set; }

        [StringLength(100)]
        [Display(Name = "Tracking Number")]
        public string? TrackingNumber { get; set; }

        [Display(Name = "Estimated Delivery")]
        [DataType(DataType.Date)]
        public DateTime? EstimatedDelivery { get; set; }

        [Display(Name = "Shipping Status")]
        public ShippingStatus Status { get; set; }
    }
}
