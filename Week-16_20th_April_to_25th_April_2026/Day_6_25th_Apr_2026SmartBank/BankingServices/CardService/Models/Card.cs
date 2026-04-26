using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CardService.Models;

public enum CardType   { Debit, Credit }
public enum CardStatus { Active, Blocked, Expired, Inactive }

public class Card
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid CustomerId { get; set; }

    [Required]
    public Guid AccountId { get; set; }

    public CardType   CardType   { get; set; }
    public CardStatus CardStatus { get; set; } = CardStatus.Inactive;

    [MaxLength(20)]
    public string CardNumber { get; set; } = string.Empty;   // masked: **** **** **** 1234

    [MaxLength(100)]
    public string CardHolderName { get; set; } = string.Empty;

    public string Network { get; set; } = "Visa";  // Visa | Mastercard

    public DateTime IssuedAt  { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? CreditLimit { get; set; }   // null for Debit

    public bool IsPinSet { get; set; } = false;

    public string? BlockReason { get; set; }

    public DateTime? BlockedAt { get; set; }
}
