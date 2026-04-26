using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoanService.Models;

public enum EmiStatus
{
    Pending,
    Paid,
    Overdue,
    Waived
}

public class EmiPlan
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid LoanId { get; set; }

    public int InstallmentNumber { get; set; }

    public DateTime DueDate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal EmiAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal PrincipalComponent { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal InterestComponent { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal OutstandingBalance { get; set; }

    public EmiStatus Status { get; set; } = EmiStatus.Pending;

    public DateTime? PaidAt { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? PaidAmount { get; set; }

    // Navigation
    public Loan Loan { get; set; } = null!;
}
