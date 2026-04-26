using System.ComponentModel.DataAnnotations;

namespace SmartHealthcare.Models.DTOs;

public class BillDTO
{
    public int Id { get; set; }
    public int AppointmentId { get; set; }
    public decimal ConsultationFee { get; set; }
    public decimal MedicineCharges { get; set; }
    public decimal TotalAmount { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
    public DateTime? PaidAt { get; set; }
    public string? TransactionReference { get; set; }
}

public class CreateBillDTO
{
    [Required]
    public int AppointmentId { get; set; }

    [Range(0, 1000000)]
    public decimal ConsultationFee { get; set; }

    [Range(0, 1000000)]
    public decimal MedicineCharges { get; set; }

    [Required]
    [RegularExpression("Paid|Unpaid")]
    public string PaymentStatus { get; set; } = "Unpaid";
}

public class UpdateBillDTO
{
    [Range(0, 1000000)]
    public decimal ConsultationFee { get; set; }

    [Range(0, 1000000)]
    public decimal MedicineCharges { get; set; }

    [Required]
    [RegularExpression("Paid|Unpaid")]
    public string PaymentStatus { get; set; } = "Unpaid";
}

public class PayBillDTO
{
    [MaxLength(100)]
    public string? TransactionReference { get; set; }
}
