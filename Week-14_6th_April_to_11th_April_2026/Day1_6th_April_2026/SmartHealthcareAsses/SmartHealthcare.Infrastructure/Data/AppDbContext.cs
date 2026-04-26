using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartHealthcare.Core.Entities;

namespace SmartHealthcare.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Department>                   Departments                  => Set<Department>();
    public DbSet<PatientProfile>               PatientProfiles              => Set<PatientProfile>();
    public DbSet<DoctorProfile>                DoctorProfiles               => Set<DoctorProfile>();
    public DbSet<Specialization>               Specializations              => Set<Specialization>();
    public DbSet<DoctorSpecializationMapping>  DoctorSpecializationMappings => Set<DoctorSpecializationMapping>();
    public DbSet<Appointment>                  Appointments                 => Set<Appointment>();
    public DbSet<Prescription>                 Prescriptions                => Set<Prescription>();
    public DbSet<Bill>                         Bills                        => Set<Bill>();
    public DbSet<Medicine>                     Medicines                    => Set<Medicine>();
    public DbSet<PrescriptionMedicine>         PrescriptionMedicines        => Set<PrescriptionMedicine>();
    public DbSet<DoctorSchedule>               DoctorSchedules              => Set<DoctorSchedule>();
    public DbSet<RefreshToken>                 RefreshTokens                => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // ── Rename Identity tables ──────────────────────────────────────────
        builder.Entity<ApplicationUser>().ToTable("Users");
        builder.Entity<IdentityRole<int>>().ToTable("Roles");
        builder.Entity<IdentityUserRole<int>>().ToTable("UserRoles");
        builder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");
        builder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");
        builder.Entity<IdentityUserToken<int>>().ToTable("UserTokens");

        // ── ONE-TO-ONE: User → PatientProfile ─────────────────────────────
        builder.Entity<ApplicationUser>()
            .HasOne(u => u.PatientProfile)
            .WithOne(p => p.User)
            .HasForeignKey<PatientProfile>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // ── ONE-TO-ONE: User → DoctorProfile ──────────────────────────────
        builder.Entity<ApplicationUser>()
            .HasOne(u => u.DoctorProfile)
            .WithOne(d => d.User)
            .HasForeignKey<DoctorProfile>(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // ── ONE-TO-MANY: Department → Doctors ─────────────────────────────
        builder.Entity<Department>()
            .HasMany(dep => dep.Doctors)
            .WithOne(d => d.Department)
            .HasForeignKey(d => d.DepartmentId)
            .OnDelete(DeleteBehavior.SetNull);

        // ── ONE-TO-MANY: Doctor → Appointments ────────────────────────────
        builder.Entity<DoctorProfile>()
            .HasMany(d => d.Appointments)
            .WithOne(a => a.DoctorProfile)
            .HasForeignKey(a => a.DoctorProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        // ── ONE-TO-MANY: Patient → Appointments ───────────────────────────
        builder.Entity<PatientProfile>()
            .HasMany(p => p.Appointments)
            .WithOne(a => a.PatientProfile)
            .HasForeignKey(a => a.PatientProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        // ── ONE-TO-ONE: Appointment → Prescription ────────────────────────
        builder.Entity<Appointment>()
            .HasOne(a => a.Prescription)
            .WithOne(p => p.Appointment)
            .HasForeignKey<Prescription>(p => p.AppointmentId)
            .OnDelete(DeleteBehavior.Cascade);

        // ── ONE-TO-ONE: Appointment → Bill ────────────────────────────────
        builder.Entity<Appointment>()
            .HasOne(a => a.Bill)
            .WithOne(b => b.Appointment)
            .HasForeignKey<Bill>(b => b.AppointmentId)
            .OnDelete(DeleteBehavior.Cascade);

        // ── MANY-TO-MANY: Doctor ↔ Specialization ─────────────────────────
        builder.Entity<DoctorSpecializationMapping>()
            .HasKey(ds => new { ds.DoctorProfileId, ds.SpecializationId });

        builder.Entity<DoctorSpecializationMapping>()
            .HasOne(ds => ds.DoctorProfile)
            .WithMany(d => d.DoctorSpecializations)
            .HasForeignKey(ds => ds.DoctorProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<DoctorSpecializationMapping>()
            .HasOne(ds => ds.Specialization)
            .WithMany(s => s.DoctorSpecializations)
            .HasForeignKey(ds => ds.SpecializationId)
            .OnDelete(DeleteBehavior.Cascade);

        // ── MANY-TO-MANY: Prescription ↔ Medicine ─────────────────────────
        builder.Entity<PrescriptionMedicine>()
            .HasKey(pm => new { pm.PrescriptionId, pm.MedicineId });

        builder.Entity<PrescriptionMedicine>()
            .HasOne(pm => pm.Prescription)
            .WithMany(p => p.PrescriptionMedicines)
            .HasForeignKey(pm => pm.PrescriptionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<PrescriptionMedicine>()
            .HasOne(pm => pm.Medicine)
            .WithMany(m => m.PrescriptionMedicines)
            .HasForeignKey(pm => pm.MedicineId)
            .OnDelete(DeleteBehavior.Restrict);

        // ── Doctor → Schedules ─────────────────────────────────────────────
        builder.Entity<DoctorSchedule>()
            .HasOne(s => s.DoctorProfile)
            .WithMany(d => d.Schedules)
            .HasForeignKey(s => s.DoctorProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        // ── RefreshToken → User ────────────────────────────────────────────
        builder.Entity<RefreshToken>()
            .HasOne(r => r.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // ── Column types ───────────────────────────────────────────────────
        builder.Entity<DoctorProfile>()
            .Property(d => d.ConsultationFee).HasColumnType("decimal(10,2)");
        builder.Entity<Appointment>()
            .Property(a => a.Fee).HasColumnType("decimal(10,2)");
        builder.Entity<PatientProfile>()
            .Property(p => p.Weight).HasColumnType("decimal(5,2)");
        builder.Entity<PatientProfile>()
            .Property(p => p.Height).HasColumnType("decimal(5,2)");
        builder.Entity<Bill>()
            .Property(b => b.ConsultationFee).HasColumnType("decimal(10,2)");
        builder.Entity<Bill>()
            .Property(b => b.MedicineCharges).HasColumnType("decimal(10,2)");
        // TotalAmount is computed in C# — ignore it in EF
        builder.Entity<Bill>()
            .Ignore(b => b.TotalAmount);

        // ── Soft-delete query filters ──────────────────────────────────────
        builder.Entity<Department>().HasQueryFilter(d => !d.IsDeleted);
        builder.Entity<PatientProfile>().HasQueryFilter(p => !p.IsDeleted);
        builder.Entity<DoctorProfile>().HasQueryFilter(d => !d.IsDeleted);
        builder.Entity<Appointment>().HasQueryFilter(a => !a.IsDeleted);
        builder.Entity<Medicine>().HasQueryFilter(m => !m.IsDeleted);
        builder.Entity<Specialization>().HasQueryFilter(s => !s.IsDeleted);
        builder.Entity<Bill>().HasQueryFilter(b => !b.IsDeleted);

        // ── Indexes ────────────────────────────────────────────────────────
        builder.Entity<ApplicationUser>()
            .HasIndex(u => u.Email).IsUnique();
        builder.Entity<DoctorProfile>()
            .HasIndex(d => d.LicenseNumber).IsUnique();
        builder.Entity<Appointment>()
            .HasIndex(a => new { a.DoctorProfileId, a.AppointmentDate, a.StartTime });
        builder.Entity<Department>()
            .HasIndex(d => d.DepartmentName).IsUnique();

        // ── Seed Roles ─────────────────────────────────────────────────────
        builder.Entity<IdentityRole<int>>().HasData(
            new IdentityRole<int> { Id = 1, Name = "Admin",   NormalizedName = "ADMIN",   ConcurrencyStamp = "c17d2c6d-222e-488c-a370-e009c9f87d38" },
            new IdentityRole<int> { Id = 2, Name = "Doctor",  NormalizedName = "DOCTOR",  ConcurrencyStamp = "af55d28c-f4db-4596-840f-4c427c9d1b26" },
            new IdentityRole<int> { Id = 3, Name = "Patient", NormalizedName = "PATIENT", ConcurrencyStamp = "784b095f-aecd-4ea2-8411-cf1a20a46cb5" }
        );

        // ── Seed Departments ───────────────────────────────────────────────
        builder.Entity<Department>().HasData(
            new Department { Id = 1, DepartmentName = "Cardiology",     Description = "Heart and cardiovascular system", CreatedAt = new DateTime(2024,1,1) },
            new Department { Id = 2, DepartmentName = "Neurology",      Description = "Brain, spine and nervous system", CreatedAt = new DateTime(2024,1,1) },
            new Department { Id = 3, DepartmentName = "Orthopedics",    Description = "Bones, joints and muscles",       CreatedAt = new DateTime(2024,1,1) },
            new Department { Id = 4, DepartmentName = "Pediatrics",     Description = "Medical care for children",       CreatedAt = new DateTime(2024,1,1) },
            new Department { Id = 5, DepartmentName = "General Medicine",Description = "General and internal medicine",  CreatedAt = new DateTime(2024,1,1) }
        );
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Modified)
                entry.Entity.UpdatedAt = DateTime.UtcNow;
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}
