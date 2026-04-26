using Microsoft.EntityFrameworkCore;
using CardService.Data;
using CardService.DTOs;
using CardService.Models;
using System.Security.Cryptography;
using System.Text;

namespace CardService.Services;

public class CardServiceImpl : ICardService
{
    private readonly CardDbContext _db;
    private readonly ILogger<CardServiceImpl> _logger;

    public CardServiceImpl(CardDbContext db, ILogger<CardServiceImpl> logger)
    {
        _db    = db;
        _logger = logger;
    }

    public async Task<CardResponse> IssueDebitCardAsync(IssueDebitCardRequest req)
    {
        var card = new Card
        {
            CustomerId     = req.CustomerId,
            AccountId      = req.AccountId,
            CardType       = CardType.Debit,
            CardStatus     = CardStatus.Active,
            CardHolderName = req.CardHolderName,
            Network        = req.Network,
            CardNumber     = GenerateMaskedCardNumber(),
            IssuedAt       = DateTime.UtcNow,
            ExpiresAt      = DateTime.UtcNow.AddYears(5)
        };

        _db.Cards.Add(card);
        await _db.SaveChangesAsync();
        _logger.LogInformation("Debit card {CardId} issued for customer {CustomerId}", card.Id, req.CustomerId);

        return CardMapper.ToResponse(card);
    }

    public async Task<CardResponse> IssueCreditCardAsync(IssueCreditCardRequest req)
    {
        var card = new Card
        {
            CustomerId     = req.CustomerId,
            AccountId      = req.AccountId,
            CardType       = CardType.Credit,
            CardStatus     = CardStatus.Active,
            CardHolderName = req.CardHolderName,
            Network        = req.Network,
            CreditLimit    = req.CreditLimit,
            CardNumber     = GenerateMaskedCardNumber(),
            IssuedAt       = DateTime.UtcNow,
            ExpiresAt      = DateTime.UtcNow.AddYears(3)
        };

        _db.Cards.Add(card);
        await _db.SaveChangesAsync();
        _logger.LogInformation("Credit card {CardId} issued for customer {CustomerId}", card.Id, req.CustomerId);

        return CardMapper.ToResponse(card);
    }

    public async Task<CardResponse> BlockCardAsync(BlockCardRequest req, string performedBy)
    {
        var card = await _db.Cards.FindAsync(req.CardId)
                   ?? throw new KeyNotFoundException($"Card {req.CardId} not found.");

        if (card.CardStatus == CardStatus.Blocked)
            throw new InvalidOperationException("Card is already blocked.");

        card.CardStatus  = CardStatus.Blocked;
        card.BlockReason = req.Reason;
        card.BlockedAt   = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        _logger.LogWarning("Card {CardId} blocked by {User}. Reason: {Reason}", card.Id, performedBy, req.Reason);

        return CardMapper.ToResponse(card);
    }

    public async Task<CardResponse> ResetPinAsync(PinResetRequest req)
    {
        var card = await _db.Cards.FindAsync(req.CardId)
                   ?? throw new KeyNotFoundException($"Card {req.CardId} not found.");

        if (card.CardStatus != CardStatus.Active)
            throw new InvalidOperationException("PIN reset is only allowed for active cards.");

        // In production: hash PIN with a proper KDF (e.g. PBKDF2). This stores a SHA-256 hash for demo.
        _ = HashPin(req.NewPin); // hash stored in a PIN vault in real impl; not persisted here for simplicity
        card.IsPinSet = true;

        await _db.SaveChangesAsync();
        _logger.LogInformation("PIN reset for card {CardId}", card.Id);

        return CardMapper.ToResponse(card);
    }

    public async Task<CardResponse> GetByIdAsync(Guid cardId)
    {
        var card = await _db.Cards.FindAsync(cardId)
                   ?? throw new KeyNotFoundException($"Card {cardId} not found.");
        return CardMapper.ToResponse(card);
    }

    public async Task<IEnumerable<CardResponse>> GetByCustomerAsync(Guid customerId)
    {
        var cards = await _db.Cards
                        .Where(c => c.CustomerId == customerId)
                        .OrderByDescending(c => c.IssuedAt)
                        .ToListAsync();
        return cards.Select(CardMapper.ToResponse);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static string GenerateMaskedCardNumber()
    {
        var last4 = RandomNumberGenerator.GetInt32(1000, 9999).ToString();
        return $"**** **** **** {last4}";
    }

    private static string HashPin(string pin)
    {
        var bytes = Encoding.UTF8.GetBytes(pin);
        var hash  = SHA256.HashData(bytes);
        return Convert.ToHexString(hash);
    }
}
