using Microsoft.EntityFrameworkCore;
using StudentPortal.Models;

namespace StudentPortal.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    "Server=Blackpearl\\SQLEXPRESS;Database=StudentPortalDb;Trusted_Connection=True;TrustServerCertificate=True");
            }
        }

        public DbSet<Student> Students { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed some initial students
            modelBuilder.Entity<Student>().HasData(
                new Student { StudentId = 1, FullName = "Alice Johnson",  Email = "alice@university.edu",  EnrollmentDate = new DateTime(2023, 9, 1) },
                new Student { StudentId = 2, FullName = "Bob Smith",      Email = "bob@university.edu",    EnrollmentDate = new DateTime(2023, 9, 1) },
                new Student { StudentId = 3, FullName = "Carol Williams", Email = "carol@university.edu",  EnrollmentDate = new DateTime(2024, 1, 15) },
                new Student { StudentId = 4, FullName = "David Brown",    Email = "david@university.edu",  EnrollmentDate = new DateTime(2024, 1, 15) }
            );
        }
    }
}
