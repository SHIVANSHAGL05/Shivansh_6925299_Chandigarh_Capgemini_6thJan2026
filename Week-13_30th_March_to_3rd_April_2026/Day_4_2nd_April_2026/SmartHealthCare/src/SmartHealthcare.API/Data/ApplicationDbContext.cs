using Microsoft.EntityFrameworkCore;
using SmartHealthcare.Models.Entities;

namespace SmartHealthcare.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Prescription> Prescriptions => Set<Prescription>();
    public DbSet<Bill> Bills => Set<Bill>();
    public DbSet<Medicine> Medicines => Set<Medicine>();
    public DbSet<Specialization> Specializations => Set<Specialization>();
    public DbSet<DoctorSpecialization> DoctorSpecializations => Set<DoctorSpecialization>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        modelBuilder.Entity<Department>().HasData(
            new Department { Id = 1, DepartmentName = "Cardiology", Description = "Heart and cardiovascular care" },
            new Department { Id = 2, DepartmentName = "Dermatology", Description = "Skin and hair treatment" },
            new Department { Id = 3, DepartmentName = "Neurology", Description = "Brain and nervous system" },
            new Department { Id = 4, DepartmentName = "Orthopedics", Description = "Bones and joints" },
            new Department { Id = 5, DepartmentName = "General Medicine", Description = "Primary and general care" }
        );

        modelBuilder.Entity<Specialization>().HasData(
            new Specialization { Id = 1, Name = "Cardiology", Description = "Heart and cardiovascular system" },
            new Specialization { Id = 2, Name = "Dermatology", Description = "Skin conditions" },
            new Specialization { Id = 3, Name = "Neurology", Description = "Nervous system disorders" },
            new Specialization { Id = 4, Name = "Orthopedics", Description = "Musculoskeletal system" },
            new Specialization { Id = 5, Name = "Pediatrics", Description = "Children's health" },
            new Specialization { Id = 6, Name = "General Medicine", Description = "General health care" }
        );

        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                FullName = "System Admin",
                Email = "admin@healthcare.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = "Admin",
                CreatedAt = DateTime.UtcNow
            }
        );
    }
}
