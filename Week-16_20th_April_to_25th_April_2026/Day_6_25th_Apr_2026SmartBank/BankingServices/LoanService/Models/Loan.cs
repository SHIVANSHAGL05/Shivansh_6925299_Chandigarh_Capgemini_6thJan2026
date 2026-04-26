using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoanService.Models;

public enum LoanStatus
{
    Pending,
    Approved,
    Rejected,
    Active,
    Closed
}

public class Loan
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid CustomerId { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal PrincipalAmount { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal InterestRatePercent { get; set; }

    public int TenureMonths { get; set; }

    public LoanStatus Status { get; set; } = LoanStatus.Pending;

    public string? Purpose { get; set; }

    public string? RejectionReason { get; set; }

    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ApprovedAt { get; set; }

    public DateTime? DisbursedAt { get; set; }

    public string? ApprovedBy { get; set; }

    // Navigation
    public ICollection<EmiPlan> EmiPlans { get; set; } = new List<EmiPlan>();
}
