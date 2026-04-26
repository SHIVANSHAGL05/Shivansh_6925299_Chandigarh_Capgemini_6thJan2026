using LoanService.Models;
using System.ComponentModel.DataAnnotations;

namespace LoanService.DTOs;

// ── Request DTOs ──────────────────────────────────────────────────────────────

public record ApplyLoanRequest(
    [Required] Guid CustomerId,
    [Required][Range(1000, 10_000_000)] decimal PrincipalAmount,
    [Required][Range(1, 360)] int TenureMonths,
    [Required][Range(1, 50)] decimal InterestRatePercent,
    string? Purpose
);

public record ApproveLoanRequest(
    [Required] Guid LoanId,
    [Required] bool Approve,
    string? RejectionReason
);

public record RepayEmiRequest(
    [Required] Guid EmiPlanId,
    [Required][Range(0.01, double.MaxValue)] decimal AmountPaid
);

// ── Response DTOs ─────────────────────────────────────────────────────────────

public record LoanResponse(
    Guid Id,
    Guid CustomerId,
    decimal PrincipalAmount,
    decimal InterestRatePercent,
    int TenureMonths,
    string Status,
    string? Purpose,
    string? RejectionReason,
    DateTime AppliedAt,
    DateTime? ApprovedAt,
    string? ApprovedBy
);

public record EmiPlanResponse(
    Guid Id,
    Guid LoanId,
    int InstallmentNumber,
    DateTime DueDate,
    decimal EmiAmount,
    decimal PrincipalComponent,
    decimal InterestComponent,
    decimal OutstandingBalance,
    string Status,
    DateTime? PaidAt,
    decimal? PaidAmount
);

public record LoanWithEmiResponse(
    LoanResponse Loan,
    IEnumerable<EmiPlanResponse> Schedule
);

// ── Mappers ───────────────────────────────────────────────────────────────────

public static class LoanMapper
{
    public static LoanResponse ToResponse(Loan l) => new(
        l.Id, l.CustomerId, l.PrincipalAmount,
        l.InterestRatePercent, l.TenureMonths,
        l.Status.ToString(), l.Purpose, l.RejectionReason,
        l.AppliedAt, l.ApprovedAt, l.ApprovedBy
    );

    public static EmiPlanResponse ToResponse(EmiPlan e) => new(
        e.Id, e.LoanId, e.InstallmentNumber, e.DueDate,
        e.EmiAmount, e.PrincipalComponent, e.InterestComponent,
        e.OutstandingBalance, e.Status.ToString(), e.PaidAt, e.PaidAmount
    );
}
