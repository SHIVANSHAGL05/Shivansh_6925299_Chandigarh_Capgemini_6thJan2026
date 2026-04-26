using Microsoft.EntityFrameworkCore;
using LoanService.Models;

namespace LoanService.Data;

public class LoanDbContext : DbContext
{
    public LoanDbContext(DbContextOptions<LoanDbContext> options) : base(options) { }

    public DbSet<Loan> Loans => Set<Loan>();
    public DbSet<EmiPlan> EmiPlans => Set<EmiPlan>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Loan>(e =>
        {
            e.ToTable("Loans");
            e.HasIndex(l => l.CustomerId);
            e.Property(l => l.Status).HasConversion<string>();
        });

        modelBuilder.Entity<EmiPlan>(e =>
        {
            e.ToTable("EmiPlans");
            e.HasOne(ep => ep.Loan)
             .WithMany(l => l.EmiPlans)
             .HasForeignKey(ep => ep.LoanId)
             .OnDelete(DeleteBehavior.Cascade);
            e.Property(ep => ep.Status).HasConversion<string>();
        });
    }
}
