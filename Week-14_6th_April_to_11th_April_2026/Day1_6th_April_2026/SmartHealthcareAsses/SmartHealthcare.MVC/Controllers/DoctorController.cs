using SmartHealthcare.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using SmartHealthcare.Core.Common;
using SmartHealthcare.Core.DTOs;
using SmartHealthcare.MVC.Services;

namespace SmartHealthcare.MVC.Controllers;

public class DoctorController : Controller
{
    private readonly IApiService _api;

    public DoctorController(IApiService api) => _api = api;

    public async Task<IActionResult> Index(int page = 1, string? search = null, int? specializationId = null)
    {
        var q = $"api/doctors?page={page}&pageSize=12";
        if (!string.IsNullOrEmpty(search))    q += $"&search={Uri.EscapeDataString(search)}";
        if (specializationId.HasValue)        q += $"&specializationId={specializationId}";

        var doctors = await _api.GetAsync<PagedResult<DoctorDto>>(q);
        var specs   = await _api.GetAsync<List<SpecializationDto>>("api/specializations");

        ViewBag.Specializations  = specs?.Data ?? new List<SpecializationDto>();
        ViewBag.CurrentPage      = page;
        ViewBag.TotalPages       = doctors?.Data?.TotalPages ?? 1;
        ViewBag.Search           = search;
        ViewBag.SpecializationId = specializationId;
        return View(doctors?.Data?.Items ?? new());
    }

    public async Task<IActionResult> Details(int id)
    {
        var doctor = await _api.GetAsync<DoctorDto>($"api/doctors/{id}");
        if (doctor?.Data == null) return NotFound();
        return View(doctor.Data);
    }

    [HttpGet]
    public async Task<IActionResult> EditProfile()
    {
        if (HttpContext.Session.GetString("JwtToken") == null)
            return RedirectToAction("Login", "Account");

        var result = await _api.GetAsync<DoctorDto>("api/doctors/me");
        if (result?.Data == null) return NotFound();

        var specs = await _api.GetAsync<List<SpecializationDto>>("api/specializations");
        ViewBag.AllSpecializations = specs?.Data ?? new List<SpecializationDto>();

        var dto = new UpdateDoctorDto
        {
            LicenseNumber     = result.Data.LicenseNumber,
            Qualifications    = result.Data.Qualifications,
            ExperienceYears   = result.Data.ExperienceYears,
            ConsultationFee   = result.Data.ConsultationFee,
            Biography         = result.Data.Biography,
            IsAvailable       = result.Data.IsAvailable,
            SpecializationIds = result.Data.Specializations.Select(s => s.Id).ToList()
        };
        ViewBag.DoctorId = result.Data.Id;
        return View(dto);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProfile(int doctorId, UpdateDoctorDto dto)
    {
        if (!ModelState.IsValid)
        {
            var specs = await _api.GetAsync<List<SpecializationDto>>("api/specializations");
            ViewBag.AllSpecializations = specs?.Data ?? new List<SpecializationDto>();
            ViewBag.DoctorId = doctorId;
            return View(dto);
        }
        var result = await _api.PutAsync<DoctorDto>($"api/doctors/{doctorId}", dto);
        if (result?.Success == true)
        {
            TempData["Success"] = "Profile updated successfully.";
            return RedirectToAction("Index", "Home");
        }
        ModelState.AddModelError(string.Empty, result?.Message ?? "Update failed.");
        var allSpecs = await _api.GetAsync<List<SpecializationDto>>("api/specializations");
        ViewBag.AllSpecializations = allSpecs?.Data ?? new List<SpecializationDto>();
        ViewBag.DoctorId = doctorId;
        return View(dto);
    }

    public async Task<IActionResult> SearchByDepartment(int? departmentId = null)
    {
        var depts   = await _api.GetAsync<List<DepartmentDto>>("api/departments");
        var doctors = new List<DoctorDto>();
        if (departmentId.HasValue)
        {
            var result = await _api.GetAsync<List<DoctorDto>>($"api/departments/{departmentId}/doctors");
            doctors = result?.Data ?? new();
        }
        var vm = new DoctorSearchViewModel
        {
            Departments          = depts?.Data ?? new(),
            Doctors              = doctors,
            SelectedDepartmentId = departmentId
        };
        return View(vm);
    }

    // ── Billing: Doctor creates a bill for a completed appointment ──────────
    [HttpGet]
    public async Task<IActionResult> CreateBill(int appointmentId)
    {
        if (HttpContext.Session.GetString("JwtToken") == null)
            return RedirectToAction("Login", "Account");

        var appt = await _api.GetAsync<AppointmentDto>($"api/appointments/{appointmentId}");
        if (appt?.Data == null) return NotFound();

        ViewBag.Appointment = appt.Data;
        var dto = new CreateBillDto
        {
            AppointmentId   = appointmentId,
            ConsultationFee = appt.Data.Fee ?? 0
        };
        return View(dto);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateBill(CreateBillDto dto)
    {
        if (!ModelState.IsValid)
        {
            var appt = await _api.GetAsync<AppointmentDto>($"api/appointments/{dto.AppointmentId}");
            ViewBag.Appointment = appt?.Data;
            return View(dto);
        }
        var result = await _api.PostAsync<BillDto>("api/bills", dto);
        if (result?.Success == true)
        {
            TempData["Success"] = "Bill created successfully.";
            return RedirectToAction("Details", "Appointment", new { id = dto.AppointmentId });
        }
        ModelState.AddModelError(string.Empty, result?.Message ?? "Failed to create bill.");
        var apptRetry = await _api.GetAsync<AppointmentDto>($"api/appointments/{dto.AppointmentId}");
        ViewBag.Appointment = apptRetry?.Data;
        return View(dto);
    }
}
