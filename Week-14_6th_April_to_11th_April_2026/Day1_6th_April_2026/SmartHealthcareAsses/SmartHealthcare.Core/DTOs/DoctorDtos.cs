using System.ComponentModel.DataAnnotations;

namespace SmartHealthcare.Core.DTOs;

public class DoctorDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string LicenseNumber { get; set; } = string.Empty;
    public string? Specialization { get; set; }
    public string? Qualifications { get; set; }
    public int ExperienceYears { get; set; }
    public string? Availability { get; set; }
    public decimal ConsultationFee { get; set; }
    public string? Biography { get; set; }
    public bool IsAvailable { get; set; }

    // Department info
    public int? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }

    // Many-to-Many specializations (optional advanced)
    public List<SpecializationDto> Specializations { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class CreateDoctorDto
{
    [Required, StringLength(100)]
    public string LicenseNumber { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Specialization { get; set; }

    [StringLength(200)]
    public string? Qualifications { get; set; }

    [Range(0, 60)]
    public int ExperienceYears { get; set; }

    [StringLength(100)]
    public string? Availability { get; set; }

    [Required, Range(0, 100000)]
    public decimal ConsultationFee { get; set; }

    public string? Biography { get; set; }

    public int? DepartmentId { get; set; }

    public List<int> SpecializationIds { get; set; } = new();
}

public class UpdateDoctorDto : CreateDoctorDto
{
    public bool IsAvailable { get; set; } = true;
}

public class SpecializationDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsPrimary { get; set; }
}

public class CreateSpecializationDto
{
    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }
}
