using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CardService.DTOs;
using CardService.Services;
using System.Security.Claims;

namespace CardService.Controllers;

/// <summary>
/// Handles debit/credit card issuance, blocking and PIN reset operations.
/// </summary>
[ApiController]
[Route("api/v1/cards")]
[Authorize]
[Produces("application/json")]
public class CardController : ControllerBase
{
    private readonly ICardService _cardService;

    public CardController(ICardService cardService)
        => _cardService = cardService;

    // ── Issue Debit Card ──────────────────────────────────────────────────────

    /// <summary>Issue a new debit card for a customer.</summary>
    [HttpPost("debit")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(CardResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> IssueDebit([FromBody] IssueDebitCardRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var card = await _cardService.IssueDebitCardAsync(request);
        return CreatedAtAction(nameof(GetById), new { cardId = card.Id }, card);
    }

    // ── Issue Credit Card ─────────────────────────────────────────────────────

    /// <summary>Issue a new credit card for a customer.</summary>
    [HttpPost("credit")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(CardResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> IssueCredit([FromBody] IssueCreditCardRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var card = await _cardService.IssueCreditCardAsync(request);
        return CreatedAtAction(nameof(GetById), new { cardId = card.Id }, card);
    }

    // ── Block Card ────────────────────────────────────────────────────────────

    /// <summary>Block an active card. Admin or card owner can block.</summary>
    [HttpPost("block")]
    [Authorize(Policy = "CustomerOrAdmin")]
    [ProducesResponseType(typeof(CardResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Block([FromBody] BlockCardRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var user = User.FindFirstValue(ClaimTypes.Name) ?? "unknown";
        try
        {
            var card = await _cardService.BlockCardAsync(request, user);
            return Ok(card);
        }
        catch (KeyNotFoundException ex)      { return NotFound(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    // ── PIN Reset ─────────────────────────────────────────────────────────────

    /// <summary>Reset the PIN for a card.</summary>
    [HttpPost("pin-reset")]
    [Authorize(Policy = "CustomerOrAdmin")]
    [ProducesResponseType(typeof(CardResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ResetPin([FromBody] PinResetRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var card = await _cardService.ResetPinAsync(request);
            return Ok(card);
        }
        catch (KeyNotFoundException ex)      { return NotFound(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    // ── Get Card by ID ────────────────────────────────────────────────────────

    /// <summary>Get a card by its ID.</summary>
    [HttpGet("{cardId:guid}")]
    [ProducesResponseType(typeof(CardResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid cardId)
    {
        try
        {
            var card = await _cardService.GetByIdAsync(cardId);
            return Ok(card);
        }
        catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
    }

    // ── Get Cards by Customer ─────────────────────────────────────────────────

    /// <summary>Get all cards issued to a customer.</summary>
    [HttpGet("customer/{customerId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<CardResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByCustomer(Guid customerId)
    {
        var cards = await _cardService.GetByCustomerAsync(customerId);
        return Ok(cards);
    }
}
