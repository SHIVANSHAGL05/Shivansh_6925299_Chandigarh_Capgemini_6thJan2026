using CardService.DTOs;

namespace CardService.Services;

public interface ICardService
{
    Task<CardResponse> IssueDebitCardAsync(IssueDebitCardRequest request);
    Task<CardResponse> IssueCreditCardAsync(IssueCreditCardRequest request);
    Task<CardResponse> BlockCardAsync(BlockCardRequest request, string performedBy);
    Task<CardResponse> ResetPinAsync(PinResetRequest request);
    Task<CardResponse> GetByIdAsync(Guid cardId);
    Task<IEnumerable<CardResponse>> GetByCustomerAsync(Guid customerId);
}
