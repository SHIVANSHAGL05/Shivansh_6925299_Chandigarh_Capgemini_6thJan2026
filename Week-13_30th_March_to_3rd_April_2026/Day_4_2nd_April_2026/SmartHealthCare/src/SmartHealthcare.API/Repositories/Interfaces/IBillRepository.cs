using SmartHealthcare.Models.Entities;

namespace SmartHealthcare.API.Repositories.Interfaces;

public interface IBillRepository : IGenericRepository<Bill>
{
    Task<Bill?> GetByAppointmentIdAsync(int appointmentId);
    Task<IEnumerable<Bill>> GetAllWithDetailsAsync(int page, int pageSize);
    Task<Bill?> GetWithDetailsAsync(int id);
}
