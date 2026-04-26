using Microsoft.EntityFrameworkCore;
using StudentRegistration.Models;

namespace StudentRegistration.Data
{
    /// <summary>
    /// ApplicationDbContext is the bridge between the application and the database.
    /// - Inherits DbContext from EF Core.
    /// - Each DbSet<T> maps to a database table.
    /// - The connection string is injected via options in Program.cs.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        // Constructor receives DbContextOptions (configured in Program.cs).
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Maps to the "Students" table in the database.
        public DbSet<Student> Students { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Explicit table name (optional — EF Core would default to "Students" anyway).
            modelBuilder.Entity<Student>().ToTable("Students");

            // Enforce Name as a required column with max length at the DB level.
            modelBuilder.Entity<Student>()
                .Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(100);
        }
    }
}
