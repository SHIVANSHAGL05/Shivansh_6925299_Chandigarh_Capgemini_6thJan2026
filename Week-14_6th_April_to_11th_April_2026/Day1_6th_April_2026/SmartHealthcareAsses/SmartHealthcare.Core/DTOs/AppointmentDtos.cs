using System.ComponentModel.DataAnnotations;
using SmartHealthcare.Core.Common;
using SmartHealthcare.Core.Enums;

namespace SmartHealthcare.Core.DTOs;

public class AppointmentDto
{
    public int Id { get; set; }
    public int PatientProfileId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public int DoctorProfileId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string? DoctorSpecializations { get; set; }
    public DateTime AppointmentDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public AppointmentStatus Status { get; set; }
    public string StatusName => Status.ToString();
    public string? ReasonForVisit { get; set; }
    public string? DoctorNotes { get; set; }
    public AppointmentType Type { get; set; }
    public decimal? Fee { get; set; }
    public bool IsPaid { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool HasPrescription { get; set; }
}

public class CreateAppointmentDto
{
    [Required(ErrorMessage = "Please select a doctor.")]
    public int DoctorProfileId { get; set; }

    [Required(ErrorMessage = "Appointment date is required.")]
    [FutureDate]
    public DateTime AppointmentDate { get; set; }

    [Required(ErrorMessage = "Start time is required.")]
    public TimeSpan StartTime { get; set; }

    [StringLength(1000)]
    public string? ReasonForVisit { get; set; }

    public AppointmentType Type { get; set; } = AppointmentType.InPerson;
}

public class UpdateAppointmentDto
{
    [Required]
    [FutureDate]
    public DateTime AppointmentDate { get; set; }

    [Required]
    public TimeSpan StartTime { get; set; }

    [StringLength(1000)]
    public string? ReasonForVisit { get; set; }

    public AppointmentType Type { get; set; }
}

public class UpdateAppointmentStatusDto
{
    [Required]
    public AppointmentStatus Status { get; set; }

    [StringLength(2000)]
    public string? DoctorNotes { get; set; }
}

public class AppointmentFilterDto
{
    public DateTime? Date { get; set; }
    public AppointmentStatus? Status { get; set; }
    public int? DoctorProfileId { get; set; }
    public int? PatientProfileId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
