using Microsoft.EntityFrameworkCore;
using LoanService.Data;
using LoanService.DTOs;
using LoanService.Models;

namespace LoanService.Services;

public class LoanServiceImpl : ILoanService
{
    private readonly LoanDbContext _db;
    private readonly ILogger<LoanServiceImpl> _logger;

    public LoanServiceImpl(LoanDbContext db, ILogger<LoanServiceImpl> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<LoanResponse> ApplyAsync(ApplyLoanRequest req, string appliedBy)
    {
        var loan = new Loan
        {
            CustomerId        = req.CustomerId,
            PrincipalAmount   = req.PrincipalAmount,
            InterestRatePercent = req.InterestRatePercent,
            TenureMonths      = req.TenureMonths,
            Purpose           = req.Purpose,
            Status            = LoanStatus.Pending
        };

        _db.Loans.Add(loan);
        await _db.SaveChangesAsync();
        _logger.LogInformation("Loan {LoanId} applied by {User}", loan.Id, appliedBy);

        return LoanMapper.ToResponse(loan);
    }

    public async Task<LoanResponse> ApproveOrRejectAsync(ApproveLoanRequest req, string adminUser)
    {
        var loan = await _db.Loans.FindAsync(req.LoanId)
                   ?? throw new KeyNotFoundException($"Loan {req.LoanId} not found.");

        if (loan.Status != LoanStatus.Pending)
            throw new InvalidOperationException("Only pending loans can be approved or rejected.");

        if (req.Approve)
        {
            loan.Status     = LoanStatus.Approved;
            loan.ApprovedAt = DateTime.UtcNow;
            loan.ApprovedBy = adminUser;
            await GenerateEmiScheduleAsync(loan);
        }
        else
        {
            loan.Status          = LoanStatus.Rejected;
            loan.RejectionReason = req.RejectionReason ?? "No reason provided.";
        }

        await _db.SaveChangesAsync();
        _logger.LogInformation("Loan {LoanId} {Action} by {Admin}", loan.Id, req.Approve ? "approved" : "rejected", adminUser);

        return LoanMapper.ToResponse(loan);
    }

    public async Task<LoanWithEmiResponse> GetLoanWithScheduleAsync(Guid loanId)
    {
        var loan = await _db.Loans
                       .Include(l => l.EmiPlans)
                       .FirstOrDefaultAsync(l => l.Id == loanId)
                   ?? throw new KeyNotFoundException($"Loan {loanId} not found.");

        return new LoanWithEmiResponse(
            LoanMapper.ToResponse(loan),
            loan.EmiPlans.OrderBy(e => e.InstallmentNumber).Select(LoanMapper.ToResponse)
        );
    }

    public async Task<IEnumerable<LoanResponse>> GetByCustomerAsync(Guid customerId)
    {
        var loans = await _db.Loans
                        .Where(l => l.CustomerId == customerId)
                        .OrderByDescending(l => l.AppliedAt)
                        .ToListAsync();
        return loans.Select(LoanMapper.ToResponse);
    }

    public async Task<IEnumerable<EmiPlanResponse>> GetScheduleAsync(Guid loanId)
    {
        var exists = await _db.Loans.AnyAsync(l => l.Id == loanId);
        if (!exists) throw new KeyNotFoundException($"Loan {loanId} not found.");

        var schedule = await _db.EmiPlans
                           .Where(e => e.LoanId == loanId)
                           .OrderBy(e => e.InstallmentNumber)
                           .ToListAsync();
        return schedule.Select(LoanMapper.ToResponse);
    }

    public async Task<EmiPlanResponse> RepayEmiAsync(RepayEmiRequest req)
    {
        var emi = await _db.EmiPlans.FindAsync(req.EmiPlanId)
                  ?? throw new KeyNotFoundException($"EMI plan {req.EmiPlanId} not found.");

        if (emi.Status == EmiStatus.Paid)
            throw new InvalidOperationException("This EMI has already been paid.");

        emi.Status     = EmiStatus.Paid;
        emi.PaidAt     = DateTime.UtcNow;
        emi.PaidAmount = req.AmountPaid;

        // Check if all EMIs paid → close loan
        var loan = await _db.Loans
                       .Include(l => l.EmiPlans)
                       .FirstAsync(l => l.Id == emi.LoanId);

        if (loan.EmiPlans.All(e => e.Status == EmiStatus.Paid))
            loan.Status = LoanStatus.Closed;

        await _db.SaveChangesAsync();
        return LoanMapper.ToResponse(emi);
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    private async Task GenerateEmiScheduleAsync(Loan loan)
    {
        var monthlyRate  = loan.InterestRatePercent / 100m / 12m;
        var n            = loan.TenureMonths;
        var principal    = loan.PrincipalAmount;

        // EMI = P * r * (1+r)^n / ((1+r)^n - 1)
        var emiAmount = monthlyRate == 0
            ? principal / n
            : principal * monthlyRate * (decimal)Math.Pow((double)(1 + monthlyRate), n)
              / ((decimal)Math.Pow((double)(1 + monthlyRate), n) - 1);

        emiAmount = Math.Round(emiAmount, 2);

        var outstanding = principal;
        var baseDate    = (loan.ApprovedAt ?? DateTime.UtcNow).AddMonths(1);

        for (int i = 1; i <= n; i++)
        {
            var interest       = Math.Round(outstanding * monthlyRate, 2);
            var principalComp  = Math.Round(emiAmount - interest, 2);
            outstanding       -= principalComp;

            _db.EmiPlans.Add(new EmiPlan
            {
                LoanId              = loan.Id,
                InstallmentNumber   = i,
                DueDate             = baseDate.AddMonths(i - 1),
                EmiAmount           = emiAmount,
                PrincipalComponent  = principalComp,
                InterestComponent   = interest,
                OutstandingBalance  = Math.Max(0, Math.Round(outstanding, 2)),
                Status              = EmiStatus.Pending
            });
        }
    }
}
