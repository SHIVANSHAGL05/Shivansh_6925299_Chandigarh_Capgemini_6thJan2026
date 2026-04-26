using System.ComponentModel.DataAnnotations;
using SmartHealthcare.Core.DTOs;
using SmartHealthcare.Core.Enums;

namespace SmartHealthcare.MVC.Models;

// ── Auth ViewModels ──────────────────────────────────────────────────────────
public class LoginViewModel
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; }
    public string? ReturnUrl { get; set; }
}

public class RegisterViewModel
{
    [Required, StringLength(50)]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    [Required, StringLength(50)]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    [Required, EmailAddress]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(100, MinimumLength = 8)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [Required, Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Register as")]
    public string Role { get; set; } = "Patient";

    [Phone]
    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }

    [Required]
    [Display(Name = "Date of Birth")]
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; } = DateTime.Today.AddYears(-25);

    [Display(Name = "Gender")]
    public string? Gender { get; set; }
}

// ── Appointment ViewModels ───────────────────────────────────────────────────
public class BookAppointmentViewModel
{
    [Required(ErrorMessage = "Please select a doctor.")]
    [Display(Name = "Doctor")]
    public int DoctorProfileId { get; set; }

    [Required(ErrorMessage = "Please select an appointment date.")]
    [DataType(DataType.Date)]
    [Display(Name = "Appointment Date")]
    public DateTime AppointmentDate { get; set; } = DateTime.Today.AddDays(1);

    [Required(ErrorMessage = "Please select a time slot.")]
    [Display(Name = "Time Slot")]
    public string StartTimeStr { get; set; } = "09:00";

    [StringLength(1000)]
    [Display(Name = "Reason for Visit")]
    public string? ReasonForVisit { get; set; }

    [Display(Name = "Appointment Type")]
    public AppointmentType Type { get; set; } = AppointmentType.InPerson;

    // Populated for the dropdown
    public List<DoctorDto> Doctors { get; set; } = new();
}

public class AppointmentListViewModel
{
    public List<AppointmentDto> Appointments { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}

// ── Dashboard ViewModels ─────────────────────────────────────────────────────
public class AdminDashboardViewModel
{
    public int TotalPatients    { get; set; }
    public int TotalDoctors     { get; set; }
    public int TotalAppointments { get; set; }
    public int TodayAppointments { get; set; }
    public List<AppointmentDto> RecentAppointments { get; set; } = new();
}

public class DoctorDashboardViewModel
{
    public DoctorDto? Profile { get; set; }
    public List<AppointmentDto> TodayAppointments { get; set; } = new();
    public List<AppointmentDto> UpcomingAppointments { get; set; } = new();
    public int TotalPatientsServed { get; set; }
}

public class PatientDashboardViewModel
{
    public PatientDto? Profile { get; set; }
    public List<AppointmentDto> UpcomingAppointments { get; set; } = new();
    public List<AppointmentDto> PastAppointments { get; set; } = new();
}

// ── Department & Search ViewModels ───────────────────────────────────────────
public class DoctorSearchViewModel
{
    public List<DepartmentDto> Departments { get; set; } = new();
    public List<DoctorDto> Doctors { get; set; } = new();
    public int? SelectedDepartmentId { get; set; }
}

public class BillViewModel
{
    public List<BillDto> Bills { get; set; } = new();
    public string? FilterStatus { get; set; }
}
