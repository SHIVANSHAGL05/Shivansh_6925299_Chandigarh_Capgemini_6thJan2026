using SmartHealthcare.Models.Entities;

namespace SmartHealthcare.API.Repositories.Interfaces;

public interface IDepartmentRepository : IGenericRepository<Department>
{
    Task<IEnumerable<Department>> GetAllWithDoctorsAsync(int page, int pageSize);
    Task<Department?> GetWithDoctorsAsync(int id);
}
