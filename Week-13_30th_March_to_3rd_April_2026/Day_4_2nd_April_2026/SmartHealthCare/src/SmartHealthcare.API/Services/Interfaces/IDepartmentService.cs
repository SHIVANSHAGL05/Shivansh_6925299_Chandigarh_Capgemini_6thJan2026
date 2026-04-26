using SmartHealthcare.Models.DTOs;

namespace SmartHealthcare.API.Services.Interfaces;

public interface IDepartmentService
{
    Task<PagedResult<DepartmentDTO>> GetAllAsync(int page, int pageSize);
    Task<DepartmentDTO?> GetByIdAsync(int id);
    Task<DepartmentDTO> CreateAsync(CreateDepartmentDTO dto);
    Task<bool> UpdateAsync(int id, UpdateDepartmentDTO dto);
    Task<bool> DeleteAsync(int id);
}
