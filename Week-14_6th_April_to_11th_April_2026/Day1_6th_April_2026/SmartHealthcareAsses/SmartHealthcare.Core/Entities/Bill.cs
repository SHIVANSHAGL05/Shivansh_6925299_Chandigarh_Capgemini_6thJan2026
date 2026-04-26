using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartHealthcare.Core.Entities;

public class Bill : BaseEntity
{
    [Required]
    public int AppointmentId { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal ConsultationFee { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal MedicineCharges { get; set; }

    // Computed in C# (SQL computed column equivalent)
    public decimal TotalAmount => ConsultationFee + MedicineCharges;

    [Required, StringLength(20)]
    public string PaymentStatus { get; set; } = "Unpaid"; // Paid | Unpaid

    [StringLength(500)]
    public string? Notes { get; set; }

    // Navigation — One-to-One: Appointment → Bill
    [ForeignKey(nameof(AppointmentId))]
    public Appointment Appointment { get; set; } = null!;
}
