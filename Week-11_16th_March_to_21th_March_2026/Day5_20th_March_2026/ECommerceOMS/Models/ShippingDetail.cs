using System.ComponentModel.DataAnnotations;

namespace ECommerceOMS.Models
{
    public enum ShippingStatus
    {
        NotShipped,
        Processing,
        InTransit,
        OutForDelivery,
        Delivered,
        Failed
    }

    public class ShippingDetail
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Recipient name is required.")]
        [StringLength(100)]
        [Display(Name = "Recipient Name")]
        public string RecipientName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Shipping address is required.")]
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

        [StringLength(100)]
        [Display(Name = "Carrier")]
        public string? Carrier { get; set; }

        [StringLength(100)]
        [Display(Name = "Tracking Number")]
        public string? TrackingNumber { get; set; }

        [Display(Name = "Shipped On")]
        public DateTime? ShippedOn { get; set; }

        [Display(Name = "Estimated Delivery")]
        public DateTime? EstimatedDelivery { get; set; }

        [Display(Name = "Delivered On")]
        public DateTime? DeliveredOn { get; set; }

        [Display(Name = "Shipping Status")]
        public ShippingStatus Status { get; set; } = ShippingStatus.NotShipped;

        // FK - one-to-one
        public int OrderId { get; set; }

        // Navigation
        public Order? Order { get; set; }
    }
}
