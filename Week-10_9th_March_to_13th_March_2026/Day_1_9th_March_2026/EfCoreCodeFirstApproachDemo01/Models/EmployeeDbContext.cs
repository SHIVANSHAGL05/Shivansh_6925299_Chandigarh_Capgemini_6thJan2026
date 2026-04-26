using Microsoft.EntityFrameworkCore;

namespace EfCoreCodeFirstApproachDemo01.Models
{
    class EmployeeDBContext : DbContext
    {
        public EmployeeDBContext(DbContextOptions<EmployeeDBContext> options) : base(options)
        {
        }
        public DbSet<EmployeeModel> employees { get; set; }
    }
}
