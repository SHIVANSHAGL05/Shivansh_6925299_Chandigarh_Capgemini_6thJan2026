using Microsoft.EntityFrameworkCore;
using SecureAccountAPI.Models;

namespace SecureAccountAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Account> Accounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>().HasData(
                new Account
                {
                    Id = 1,
                    AccountHolderName = "Shivansh Agrawal",
                    AccountNumber = "9876543210123456",
                    Balance = 85000.00m,
                    UserId = "user-001"
                }
            );
        }
    }
}
