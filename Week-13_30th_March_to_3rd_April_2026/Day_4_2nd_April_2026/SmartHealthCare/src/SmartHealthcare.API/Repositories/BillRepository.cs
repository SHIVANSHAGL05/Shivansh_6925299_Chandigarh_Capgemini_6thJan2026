using Microsoft.EntityFrameworkCore;
using SmartHealthcare.API.Data;
using SmartHealthcare.API.Repositories.Interfaces;
using SmartHealthcare.Models.Entities;

namespace SmartHealthcare.API.Repositories;

public class BillRepository : GenericRepository<Bill>, IBillRepository
{
    public BillRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Bill?> GetByAppointmentIdAsync(int appointmentId)
        => await _dbSet.Include(b => b.Appointment)
                       .FirstOrDefaultAsync(b => b.AppointmentId == appointmentId);

    public async Task<IEnumerable<Bill>> GetAllWithDetailsAsync(int page, int pageSize)
        => await _dbSet.Include(b => b.Appointment)
                       .OrderByDescending(b => b.Id)
                       .Skip((page - 1) * pageSize)
                       .Take(pageSize)
                       .ToListAsync();

    public async Task<Bill?> GetWithDetailsAsync(int id)
        => await _dbSet.Include(b => b.Appointment)
                       .FirstOrDefaultAsync(b => b.Id == id);
}
