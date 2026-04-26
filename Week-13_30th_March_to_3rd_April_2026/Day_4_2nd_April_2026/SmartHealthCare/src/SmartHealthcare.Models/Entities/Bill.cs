using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartHealthcare.Models.Entities;

public class Bill
{
    public int Id { get; set; }
    public int AppointmentId { get; set; }

    [Range(0, 1000000)]
    public decimal ConsultationFee { get; set; }

    [Range(0, 1000000)]
    public decimal MedicineCharges { get; set; }

    [NotMapped]
    public decimal TotalAmount => ConsultationFee + MedicineCharges;

    [Required, MaxLength(20)]
    public string PaymentStatus { get; set; } = "Unpaid";

    public DateTime? PaidAt { get; set; }

    [MaxLength(100)]
    public string? TransactionReference { get; set; }

    public Appointment Appointment { get; set; } = null!;
}
