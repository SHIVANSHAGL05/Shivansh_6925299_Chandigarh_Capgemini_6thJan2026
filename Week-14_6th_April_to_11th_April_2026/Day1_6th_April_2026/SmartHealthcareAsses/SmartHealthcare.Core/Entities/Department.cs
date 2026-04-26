using System.ComponentModel.DataAnnotations;

namespace SmartHealthcare.Core.Entities;

public class Department : BaseEntity
{
    [Required, StringLength(100)]
    public string DepartmentName { get; set; } = string.Empty;

    [StringLength(255)]
    public string? Description { get; set; }

    // Navigation — One-to-Many: Department → Doctors
    public ICollection<DoctorProfile> Doctors { get; set; } = new List<DoctorProfile>();
}
