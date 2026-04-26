using SmartHealthcare.Models.DTOs;

namespace SmartHealthcare.API.Services.Interfaces;

public interface IBillService
{
    Task<PagedResult<BillDTO>> GetAllAsync(int page, int pageSize);
    Task<BillDTO?> GetByIdAsync(int id);
    Task<BillDTO?> GetByAppointmentIdAsync(int appointmentId);
    Task<BillDTO> CreateAsync(CreateBillDTO dto);
    Task<bool> UpdateAsync(int id, UpdateBillDTO dto);
    Task<bool> MarkAsPaidAsync(int id, PayBillDTO dto);
}
