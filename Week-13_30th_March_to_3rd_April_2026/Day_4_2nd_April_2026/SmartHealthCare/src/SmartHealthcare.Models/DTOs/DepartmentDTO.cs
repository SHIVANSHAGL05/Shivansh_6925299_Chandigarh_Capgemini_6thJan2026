using System.ComponentModel.DataAnnotations;

namespace SmartHealthcare.Models.DTOs;

public class DepartmentDTO
{
    public int Id { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DoctorsCount { get; set; }
}

public class CreateDepartmentDTO
{
    [Required]
    [MaxLength(100)]
    public string DepartmentName { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Description { get; set; }
}

public class UpdateDepartmentDTO
{
    [Required]
    [MaxLength(100)]
    public string DepartmentName { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Description { get; set; }
}
