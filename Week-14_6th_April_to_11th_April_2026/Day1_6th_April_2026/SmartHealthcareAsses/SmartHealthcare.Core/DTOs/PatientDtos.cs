using System.ComponentModel.DataAnnotations;
using SmartHealthcare.Core.Common;

namespace SmartHealthcare.Core.DTOs;

public class PatientDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? BloodGroup { get; set; }
    public string? AllergiesNotes { get; set; }
    public string? MedicalHistory { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public decimal? Weight { get; set; }
    public decimal? Height { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreatePatientDto
{
    [ValidBloodGroup]
    [StringLength(20)]
    public string? BloodGroup { get; set; }

    public string? AllergiesNotes { get; set; }
    public string? MedicalHistory { get; set; }

    [StringLength(100)]
    public string? EmergencyContactName { get; set; }

    [InternationalPhone]
    [StringLength(20)]
    public string? EmergencyContactPhone { get; set; }

    [Range(20, 300, ErrorMessage = "Weight must be between 20 and 300 kg.")]
    public decimal? Weight { get; set; }

    [Range(50, 250, ErrorMessage = "Height must be between 50 and 250 cm.")]
    public decimal? Height { get; set; }
}

public class UpdatePatientDto : CreatePatientDto { }
