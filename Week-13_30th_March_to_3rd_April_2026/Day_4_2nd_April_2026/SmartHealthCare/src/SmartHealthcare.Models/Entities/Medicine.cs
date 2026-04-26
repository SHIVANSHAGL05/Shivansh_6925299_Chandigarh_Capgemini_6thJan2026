using System.ComponentModel.DataAnnotations;

namespace SmartHealthcare.Models.Entities;

public class Medicine
{
    public int Id { get; set; }
    public int PrescriptionId { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Dosage { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string Duration { get; set; } = string.Empty;

    [MaxLength(300)]
    public string? Instructions { get; set; }

    public Prescription Prescription { get; set; } = null!;
}
