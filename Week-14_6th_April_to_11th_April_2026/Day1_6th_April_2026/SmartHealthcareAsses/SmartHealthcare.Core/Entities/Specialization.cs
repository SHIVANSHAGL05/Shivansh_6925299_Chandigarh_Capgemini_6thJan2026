using System.ComponentModel.DataAnnotations;

namespace SmartHealthcare.Core.Entities;

public class Specialization : BaseEntity
{
    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    // Navigation — Many-to-Many via join table
    public ICollection<DoctorSpecializationMapping> DoctorSpecializations { get; set; } = new List<DoctorSpecializationMapping>();
}

// Explicit join table (Many-to-Many: Doctor ↔ Specialization)
public class DoctorSpecializationMapping
{
    public int DoctorProfileId { get; set; }
    public int SpecializationId { get; set; }
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    public bool IsPrimary { get; set; } = false;

    // Navigation
    public DoctorProfile DoctorProfile { get; set; } = null!;
    public Specialization Specialization { get; set; } = null!;
}
