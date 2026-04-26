using Microsoft.EntityFrameworkCore;
using CardService.Models;

namespace CardService.Data;

public class CardDbContext : DbContext
{
    public CardDbContext(DbContextOptions<CardDbContext> options) : base(options) { }

    public DbSet<Card> Cards => Set<Card>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Card>(e =>
        {
            e.ToTable("Cards");
            e.HasIndex(c => c.CustomerId);
            e.HasIndex(c => c.CardNumber).IsUnique();
            e.Property(c => c.CardType).HasConversion<string>();
            e.Property(c => c.CardStatus).HasConversion<string>();
        });
    }
}
