using Microsoft.EntityFrameworkCore;
using RoleBasedAccessAPI.Models;

namespace RoleBasedAccessAPI.Data
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
                    AccountHolderName = "Shivansh Sharma",
                    AccountNumber = "1234567890123456",
                    Balance = 150000.00m,
                    UserId = "user-001",
                    Email = "shivansh@bank.com",
                    PhoneNumber = "9876543210"
                },
                new Account
                {
                    Id = 2,
                    AccountHolderName = "Shiva Verma",
                    AccountNumber = "6543210987654321",
                    Balance = 75000.00m,
                    UserId = "user-002",
                    Email = "shiva@bank.com",
                    PhoneNumber = "9123456780"
                }
            );
        }
    }
}
