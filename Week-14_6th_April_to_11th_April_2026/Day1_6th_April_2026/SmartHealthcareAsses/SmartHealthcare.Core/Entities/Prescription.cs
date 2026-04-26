using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartHealthcare.Core.Entities;

public class Prescription : BaseEntity
{
    [Required]
    public int AppointmentId { get; set; }

    [Required]
    public DateTime IssuedDate { get; set; } = DateTime.UtcNow;

    public DateTime? ValidUntil { get; set; }

    [StringLength(2000)]
    public string? Diagnosis { get; set; }

    [StringLength(2000)]
    public string? Instructions { get; set; }

    [StringLength(1000)]
    public string? FollowUpNotes { get; set; }

    // Navigation
    [ForeignKey(nameof(AppointmentId))]
    public Appointment Appointment { get; set; } = null!;

    public ICollection<PrescriptionMedicine> PrescriptionMedicines { get; set; } = new List<PrescriptionMedicine>();
}

public class Medicine : BaseEntity
{
    [Required, StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(100)]
    public string? GenericName { get; set; }

    [StringLength(100)]
    public string? Manufacturer { get; set; }

    [StringLength(100)]
    public string? Category { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    public bool RequiresPrescription { get; set; } = true;

    // Navigation
    public ICollection<PrescriptionMedicine> PrescriptionMedicines { get; set; } = new List<PrescriptionMedicine>();
}

// Join table for Prescription ↔ Medicine
public class PrescriptionMedicine
{
    public int PrescriptionId { get; set; }
    public int MedicineId { get; set; }

    [StringLength(200)]
    public string Dosage { get; set; } = string.Empty;   // e.g. "500mg"

    [StringLength(200)]
    public string Frequency { get; set; } = string.Empty; // e.g. "Twice daily"

    [StringLength(200)]
    public string Duration { get; set; } = string.Empty;  // e.g. "7 days"

    [StringLength(500)]
    public string? SpecialInstructions { get; set; }

    // Navigation
    public Prescription Prescription { get; set; } = null!;
    public Medicine Medicine { get; set; } = null!;
}
