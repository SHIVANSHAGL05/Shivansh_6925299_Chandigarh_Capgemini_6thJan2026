namespace StudentManagementSystem.Models
{
    /// <summary>
    /// Product entity used by the ProductController module.
    /// Data is hard-coded in-memory — no database needed for this module.
    /// </summary>
    public class Product
    {
        public int     Id       { get; set; }
        public string  Name     { get; set; } = string.Empty;
        public string  Category { get; set; } = string.Empty;
        public decimal Price    { get; set; }
        public int     Stock    { get; set; }
    }
}
