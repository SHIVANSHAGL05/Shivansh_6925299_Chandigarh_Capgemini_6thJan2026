using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartHealthcare.Core.Entities;

public class PatientProfile : BaseEntity
{
    [Required]
    public int UserId { get; set; }

    [StringLength(20)]
    public string? BloodGroup { get; set; }

    public string? AllergiesNotes { get; set; }

    public string? MedicalHistory { get; set; }

    [StringLength(20)]
    public string? EmergencyContactPhone { get; set; }

    public string? EmergencyContactName { get; set; }

    public decimal? Weight { get; set; }  // in kg

    public decimal? Height { get; set; }  // in cm

    // Navigation
    [ForeignKey(nameof(UserId))]
    public ApplicationUser User { get; set; } = null!;

    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
