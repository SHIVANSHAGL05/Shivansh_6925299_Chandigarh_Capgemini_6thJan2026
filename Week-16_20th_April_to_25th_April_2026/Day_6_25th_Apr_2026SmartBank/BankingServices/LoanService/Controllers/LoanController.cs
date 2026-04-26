using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoanService.DTOs;
using LoanService.Services;
using System.Security.Claims;

namespace LoanService.Controllers;

/// <summary>
/// Manages loan lifecycle: application, approval/rejection, EMI schedule and repayments.
/// </summary>
[ApiController]
[Route("api/v1/loans")]
[Authorize]
[Produces("application/json")]
public class LoanController : ControllerBase
{
    private readonly ILoanService _loanService;

    public LoanController(ILoanService loanService)
        => _loanService = loanService;

    // ── Apply Loan ────────────────────────────────────────────────────────────

    /// <summary>Apply for a new loan.</summary>
    /// <remarks>Customer submits loan details. Loan starts in Pending status.</remarks>
    [HttpPost("apply")]
    [Authorize(Policy = "CustomerOrAdmin")]
    [ProducesResponseType(typeof(LoanResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Apply([FromBody] ApplyLoanRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var user = User.FindFirstValue(ClaimTypes.Name) ?? "unknown";
        var loan = await _loanService.ApplyAsync(request, user);
        return CreatedAtAction(nameof(GetById), new { loanId = loan.Id }, loan);
    }

    // ── Approve / Reject ──────────────────────────────────────────────────────

    /// <summary>Approve or reject a pending loan (Admin only).</summary>
    [HttpPost("decision")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(LoanResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Decision([FromBody] ApproveLoanRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var admin = User.FindFirstValue(ClaimTypes.Name) ?? "admin";
        try
        {
            var loan = await _loanService.ApproveOrRejectAsync(request, admin);
            return Ok(loan);
        }
        catch (KeyNotFoundException ex)   { return NotFound(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    // ── Get Loan + EMI Schedule ───────────────────────────────────────────────

    /// <summary>Get a loan along with its full EMI schedule.</summary>
    [HttpGet("{loanId:guid}")]
    [ProducesResponseType(typeof(LoanWithEmiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid loanId)
    {
        try
        {
            var result = await _loanService.GetLoanWithScheduleAsync(loanId);
            return Ok(result);
        }
        catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
    }

    // ── EMI Schedule only ─────────────────────────────────────────────────────

    /// <summary>Get the EMI schedule for a loan.</summary>
    [HttpGet("{loanId:guid}/schedule")]
    [ProducesResponseType(typeof(IEnumerable<EmiPlanResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSchedule(Guid loanId)
    {
        try
        {
            var schedule = await _loanService.GetScheduleAsync(loanId);
            return Ok(schedule);
        }
        catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
    }

    // ── Get loans by customer ─────────────────────────────────────────────────

    /// <summary>Get all loans for a customer.</summary>
    [HttpGet("customer/{customerId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<LoanResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByCustomer(Guid customerId)
    {
        var loans = await _loanService.GetByCustomerAsync(customerId);
        return Ok(loans);
    }

    // ── Repay EMI ─────────────────────────────────────────────────────────────

    /// <summary>Record repayment of an EMI instalment.</summary>
    [HttpPost("repay")]
    [Authorize(Policy = "CustomerOrAdmin")]
    [ProducesResponseType(typeof(EmiPlanResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Repay([FromBody] RepayEmiRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var emi = await _loanService.RepayEmiAsync(request);
            return Ok(emi);
        }
        catch (KeyNotFoundException ex)      { return NotFound(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }
}
