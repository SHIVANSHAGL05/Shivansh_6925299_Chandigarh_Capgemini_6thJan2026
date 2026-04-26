using System.ComponentModel.DataAnnotations;

namespace SmartHealthcare.Core.DTOs;

public class BillDto
{
    public int Id { get; set; }
    public int AppointmentId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string DoctorName { get; set; } = string.Empty;
    public DateTime AppointmentDate { get; set; }
    public decimal ConsultationFee { get; set; }
    public decimal MedicineCharges { get; set; }
    public decimal TotalAmount { get; set; }
    public string PaymentStatus { get; set; } = "Unpaid";
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateBillDto
{
    [Required(ErrorMessage = "AppointmentId is required.")]
    public int AppointmentId { get; set; }

    [Range(0, 999999, ErrorMessage = "Consultation fee must be a positive value.")]
    public decimal ConsultationFee { get; set; }

    [Range(0, 999999, ErrorMessage = "Medicine charges must be a positive value.")]
    public decimal MedicineCharges { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }
}

public class UpdateBillPaymentDto
{
    [Required]
    [RegularExpression("^(Paid|Unpaid)$", ErrorMessage = "PaymentStatus must be 'Paid' or 'Unpaid'.")]
    public string PaymentStatus { get; set; } = "Unpaid";
}
