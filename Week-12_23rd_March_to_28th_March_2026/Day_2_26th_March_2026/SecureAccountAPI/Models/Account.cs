namespace SecureAccountAPI.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string AccountHolderName { get; set; }
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public string UserId { get; set; }
    }
}
