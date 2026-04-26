using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartHealthcare.Core.Entities;

public class DoctorProfile : BaseEntity
{
    [Required]
    public int UserId { get; set; }

    /// <summary>FK to Department (One-to-Many: Department → Doctors)</summary>
    public int? DepartmentId { get; set; }

    [Required, StringLength(100)]
    public string LicenseNumber { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Specialization { get; set; }

    [StringLength(200)]
    public string? Qualifications { get; set; }

    public int ExperienceYears { get; set; }

    [StringLength(100)]
    public string? Availability { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal ConsultationFee { get; set; }

    public string? Biography { get; set; }

    public bool IsAvailable { get; set; } = true;

    // Navigation
    [ForeignKey(nameof(UserId))]
    public ApplicationUser User { get; set; } = null!;

    [ForeignKey(nameof(DepartmentId))]
    public Department? Department { get; set; }

    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<DoctorSpecializationMapping> DoctorSpecializations { get; set; } = new List<DoctorSpecializationMapping>();
    public ICollection<DoctorSchedule> Schedules { get; set; } = new List<DoctorSchedule>();
}
