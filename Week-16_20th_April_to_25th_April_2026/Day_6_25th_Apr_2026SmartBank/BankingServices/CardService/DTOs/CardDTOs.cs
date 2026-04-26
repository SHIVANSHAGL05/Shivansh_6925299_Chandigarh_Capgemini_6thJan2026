using CardService.Models;
using System.ComponentModel.DataAnnotations;

namespace CardService.DTOs;

// ── Request DTOs ──────────────────────────────────────────────────────────────

public record IssueDebitCardRequest(
    [Required] Guid CustomerId,
    [Required] Guid AccountId,
    [Required][MaxLength(100)] string CardHolderName,
    string Network = "Visa"
);

public record IssueCreditCardRequest(
    [Required] Guid CustomerId,
    [Required] Guid AccountId,
    [Required][MaxLength(100)] string CardHolderName,
    [Required][Range(1000, 10_000_000)] decimal CreditLimit,
    string Network = "Visa"
);

public record BlockCardRequest(
    [Required] Guid CardId,
    [Required][MaxLength(255)] string Reason
);

public record PinResetRequest(
    [Required] Guid CardId,
    [Required][MinLength(4)][MaxLength(6)] string NewPin  // hashed before storage
);

// ── Response DTOs ─────────────────────────────────────────────────────────────

public record CardResponse(
    Guid     Id,
    Guid     CustomerId,
    Guid     AccountId,
    string   CardType,
    string   CardStatus,
    string   CardNumber,          // masked
    string   CardHolderName,
    string   Network,
    DateTime IssuedAt,
    DateTime ExpiresAt,
    decimal? CreditLimit,
    bool     IsPinSet,
    string?  BlockReason,
    DateTime? BlockedAt
);

// ── Mapper ────────────────────────────────────────────────────────────────────

public static class CardMapper
{
    public static CardResponse ToResponse(Card c) => new(
        c.Id, c.CustomerId, c.AccountId,
        c.CardType.ToString(), c.CardStatus.ToString(),
        c.CardNumber, c.CardHolderName, c.Network,
        c.IssuedAt, c.ExpiresAt, c.CreditLimit,
        c.IsPinSet, c.BlockReason, c.BlockedAt
    );
}
