using System.ComponentModel.DataAnnotations;

namespace SmartHealthcare.Models.Entities;

public class Department
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string DepartmentName { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Description { get; set; }

    public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
}
