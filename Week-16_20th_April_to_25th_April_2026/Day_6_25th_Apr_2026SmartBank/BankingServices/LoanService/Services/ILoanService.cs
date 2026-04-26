using LoanService.DTOs;

namespace LoanService.Services;

public interface ILoanService
{
    Task<LoanResponse> ApplyAsync(ApplyLoanRequest request, string appliedBy);
    Task<LoanResponse> ApproveOrRejectAsync(ApproveLoanRequest request, string adminUser);
    Task<LoanWithEmiResponse> GetLoanWithScheduleAsync(Guid loanId);
    Task<IEnumerable<LoanResponse>> GetByCustomerAsync(Guid customerId);
    Task<EmiPlanResponse> RepayEmiAsync(RepayEmiRequest request);
    Task<IEnumerable<EmiPlanResponse>> GetScheduleAsync(Guid loanId);
}
