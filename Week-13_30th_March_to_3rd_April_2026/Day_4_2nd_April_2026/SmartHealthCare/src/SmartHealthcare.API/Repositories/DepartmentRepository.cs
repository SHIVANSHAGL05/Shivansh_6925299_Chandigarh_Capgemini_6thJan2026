using Microsoft.EntityFrameworkCore;
using SmartHealthcare.API.Data;
using SmartHealthcare.API.Repositories.Interfaces;
using SmartHealthcare.Models.Entities;

namespace SmartHealthcare.API.Repositories;

public class DepartmentRepository : GenericRepository<Department>, IDepartmentRepository
{
    public DepartmentRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Department>> GetAllWithDoctorsAsync(int page, int pageSize)
        => await _dbSet.Include(d => d.Doctors)
                       .OrderBy(d => d.DepartmentName)
                       .Skip((page - 1) * pageSize)
                       .Take(pageSize)
                       .ToListAsync();

    public async Task<Department?> GetWithDoctorsAsync(int id)
        => await _dbSet.Include(d => d.Doctors)
                       .FirstOrDefaultAsync(d => d.Id == id);
}
