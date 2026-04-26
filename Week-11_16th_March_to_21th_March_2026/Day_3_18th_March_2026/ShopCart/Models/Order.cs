using System.ComponentModel.DataAnnotations;

namespace ShopCart.Models
{
    public class Order
    {
        public int    Id         { get; set; }
        public string OrderRef   { get; set; } = "";
        public DateTime PlacedOn { get; set; } = DateTime.Now;
        public decimal Total     { get; set; }

        // Address
        [Required(ErrorMessage = "Full name is required")]
        public string FullName   { get; set; } = "";

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email")]
        public string Email      { get; set; } = "";

        [Required(ErrorMessage = "Phone is required")]
        public string Phone      { get; set; } = "";

        [Required(ErrorMessage = "Address is required")]
        public string Address    { get; set; } = "";

        [Required(ErrorMessage = "City is required")]
        public string City       { get; set; } = "";

        [Required(ErrorMessage = "PIN code is required")]
        public string PinCode    { get; set; } = "";

        // Payment
        [Required(ErrorMessage = "Card number is required")]
        [StringLength(16, MinimumLength = 16, ErrorMessage = "Card number must be 16 digits")]
        public string CardNumber { get; set; } = "";

        [Required(ErrorMessage = "Cardholder name is required")]
        public string CardName   { get; set; } = "";

        [Required(ErrorMessage = "Expiry is required")]
        public string CardExpiry { get; set; } = "";

        [Required(ErrorMessage = "CVV is required")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "CVV must be 3 digits")]
        public string CardCvv    { get; set; } = "";

        // Items in the order
        public List<CartItem> Items { get; set; } = new();
    }
}
