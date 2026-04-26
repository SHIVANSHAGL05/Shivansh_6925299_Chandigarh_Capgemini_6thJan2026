using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SmartHealthcare.Core.Enums;

namespace SmartHealthcare.Core.Entities;

public class Appointment : BaseEntity
{
    [Required]
    public int PatientProfileId { get; set; }

    [Required]
    public int DoctorProfileId { get; set; }

    [Required]
    public DateTime AppointmentDate { get; set; }

    [Required]
    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

    [StringLength(1000)]
    public string? ReasonForVisit { get; set; }

    [StringLength(2000)]
    public string? DoctorNotes { get; set; }

    public AppointmentType Type { get; set; } = AppointmentType.InPerson;

    [Column(TypeName = "decimal(10,2)")]
    public decimal? Fee { get; set; }

    public bool IsPaid { get; set; } = false;

    // Navigation
    [ForeignKey(nameof(PatientProfileId))]
    public PatientProfile PatientProfile { get; set; } = null!;

    [ForeignKey(nameof(DoctorProfileId))]
    public DoctorProfile DoctorProfile { get; set; } = null!;

    // One-to-One: Appointment → Prescription
    public Prescription? Prescription { get; set; }

    // One-to-One: Appointment → Bill
    public Bill? Bill { get; set; }
}
