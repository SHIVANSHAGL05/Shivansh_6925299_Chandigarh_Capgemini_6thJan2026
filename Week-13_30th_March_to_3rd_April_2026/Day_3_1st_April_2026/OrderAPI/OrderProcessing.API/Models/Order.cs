namespace OrderProcessing.API.Models
{
    /// <summary>
    /// Represents a customer order.
    /// </summary>
    public class Order
    {
        public int    Id           { get; set; }
        public int    CustomerId   { get; set; }
        public string ProductName  { get; set; } = string.Empty;
        public int    Quantity     { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate  { get; set; } = DateTime.UtcNow;
    }
}
