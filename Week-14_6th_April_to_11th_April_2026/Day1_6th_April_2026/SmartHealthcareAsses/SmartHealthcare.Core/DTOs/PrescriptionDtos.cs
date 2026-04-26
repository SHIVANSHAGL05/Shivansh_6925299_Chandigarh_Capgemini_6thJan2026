using System.ComponentModel.DataAnnotations;

namespace SmartHealthcare.Core.DTOs;

public class PrescriptionDto
{
    public int Id { get; set; }
    public int AppointmentId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string DoctorName { get; set; } = string.Empty;
    public DateTime IssuedDate { get; set; }
    public DateTime? ValidUntil { get; set; }
    public string? Diagnosis { get; set; }
    public string? Instructions { get; set; }
    public string? FollowUpNotes { get; set; }
    public List<PrescriptionMedicineDto> Medicines { get; set; } = new();
}

public class CreatePrescriptionDto
{
    [Required]
    public int AppointmentId { get; set; }

    public DateTime? ValidUntil { get; set; }

    [StringLength(2000)]
    public string? Diagnosis { get; set; }

    [StringLength(2000)]
    public string? Instructions { get; set; }

    [StringLength(1000)]
    public string? FollowUpNotes { get; set; }

    [Required, MinLength(1, ErrorMessage = "At least one medicine is required.")]
    public List<CreatePrescriptionMedicineDto> Medicines { get; set; } = new();
}

public class PrescriptionMedicineDto
{
    public int MedicineId { get; set; }
    public string MedicineName { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public string? SpecialInstructions { get; set; }
}

public class CreatePrescriptionMedicineDto
{
    [Required]
    public int MedicineId { get; set; }

    [Required, StringLength(200)]
    public string Dosage { get; set; } = string.Empty;

    [Required, StringLength(200)]
    public string Frequency { get; set; } = string.Empty;

    [Required, StringLength(200)]
    public string Duration { get; set; } = string.Empty;

    [StringLength(500)]
    public string? SpecialInstructions { get; set; }
}

public class MedicineDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? GenericName { get; set; }
    public string? Manufacturer { get; set; }
    public string? Category { get; set; }
    public string? Description { get; set; }
    public bool RequiresPrescription { get; set; }
}

public class CreateMedicineDto
{
    [Required, StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(100)]
    public string? GenericName { get; set; }

    [StringLength(100)]
    public string? Manufacturer { get; set; }

    [StringLength(100)]
    public string? Category { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    public bool RequiresPrescription { get; set; } = true;
}
