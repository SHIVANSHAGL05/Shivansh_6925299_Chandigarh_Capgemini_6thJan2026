using System.ComponentModel.DataAnnotations;

namespace SmartHealthcare.Core.DTOs;

public class DepartmentDto
{
    public int Id { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DoctorCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateDepartmentDto
{
    [Required(ErrorMessage = "Department name is required.")]
    [StringLength(100, ErrorMessage = "Department name cannot exceed 100 characters.")]
    public string DepartmentName { get; set; } = string.Empty;

    [StringLength(255)]
    public string? Description { get; set; }
}

public class UpdateDepartmentDto : CreateDepartmentDto { }
