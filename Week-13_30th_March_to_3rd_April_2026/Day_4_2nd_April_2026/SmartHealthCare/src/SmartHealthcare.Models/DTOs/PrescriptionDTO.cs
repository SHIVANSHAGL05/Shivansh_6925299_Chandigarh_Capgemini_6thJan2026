using System.ComponentModel.DataAnnotations;

namespace SmartHealthcare.Models.DTOs;

public class PrescriptionDTO
{
    public int Id { get; set; }
    public int AppointmentId { get; set; }
    public string Diagnosis { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<MedicineDTO> Medicines { get; set; } = new();
}

public class CreatePrescriptionDTO
{
    [Required]
    public int AppointmentId { get; set; }

    [Required(ErrorMessage = "Diagnosis is required")]
    public string Diagnosis { get; set; } = string.Empty;

    public string? Notes { get; set; }

    public List<CreateMedicineDTO> Medicines { get; set; } = new();
}

public class MedicineDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public string? Instructions { get; set; }
}

public class CreateMedicineDTO
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Dosage { get; set; } = string.Empty;

    [Required]
    public string Duration { get; set; } = string.Empty;

    public string? Instructions { get; set; }
}
