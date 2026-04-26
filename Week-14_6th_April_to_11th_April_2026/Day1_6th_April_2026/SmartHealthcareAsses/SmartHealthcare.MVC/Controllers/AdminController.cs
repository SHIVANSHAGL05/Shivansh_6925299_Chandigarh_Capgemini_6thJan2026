using Microsoft.AspNetCore.Mvc;
using SmartHealthcare.Core.Common;
using SmartHealthcare.Core.DTOs;
using SmartHealthcare.MVC.Services;

namespace SmartHealthcare.MVC.Controllers;

public class AdminController : Controller
{
    private readonly IApiService _api;
    private readonly ILogger<AdminController> _logger;

    public AdminController(IApiService api, ILogger<AdminController> logger)
    {
        _api    = api;
        _logger = logger;
    }

    private bool IsAdmin => HttpContext.Session.GetString("UserRole") == "Admin";

    private IActionResult RequireAdmin()
    {
        if (!IsAdmin) return RedirectToAction("AccessDenied", "Account");
        return null!;
    }

    // ── Patients Management ──────────────────────────────────────────────────
    public async Task<IActionResult> Patients(int page = 1, string? search = null)
    {
        var guard = RequireAdmin(); if (guard != null) return guard;

        var q = $"api/patients?page={page}&pageSize=15";
        if (!string.IsNullOrWhiteSpace(search)) q += $"&search={Uri.EscapeDataString(search)}";

        var result = await _api.GetAsync<PagedResult<PatientDto>>(q);
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages  = result?.Data?.TotalPages ?? 1;
        ViewBag.TotalCount  = result?.Data?.TotalCount ?? 0;
        ViewBag.Search      = search;
        return View(result?.Data?.Items ?? new List<PatientDto>());
    }

    public async Task<IActionResult> PatientDetails(int id)
    {
        var guard = RequireAdmin(); if (guard != null) return guard;
        var result = await _api.GetAsync<PatientDto>($"api/patients/{id}");
        if (result?.Data == null) return NotFound();
        return View(result.Data);
    }

    // ── Doctors Management ───────────────────────────────────────────────────
    public async Task<IActionResult> Doctors(int page = 1, string? search = null, int? specializationId = null)
    {
        var guard = RequireAdmin(); if (guard != null) return guard;

        var q = $"api/doctors?page={page}&pageSize=15";
        if (!string.IsNullOrWhiteSpace(search))  q += $"&search={Uri.EscapeDataString(search)}";
        if (specializationId.HasValue)            q += $"&specializationId={specializationId}";

        var result = await _api.GetAsync<PagedResult<DoctorDto>>(q);
        var specs   = await _api.GetAsync<List<SpecializationDto>>("api/specializations");

        ViewBag.CurrentPage      = page;
        ViewBag.TotalPages       = result?.Data?.TotalPages ?? 1;
        ViewBag.TotalCount       = result?.Data?.TotalCount ?? 0;
        ViewBag.Search           = search;
        ViewBag.SpecializationId = specializationId;
        ViewBag.Specializations  = specs?.Data ?? new List<SpecializationDto>();
        return View(result?.Data?.Items ?? new List<DoctorDto>());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteDoctor(int id)
    {
        var guard = RequireAdmin(); if (guard != null) return guard;
        var result = await _api.DeleteAsync<bool>($"api/doctors/{id}");
        TempData[result?.Success == true ? "Success" : "Error"] =
            result?.Success == true ? "Doctor removed." : result?.Message ?? "Failed.";
        return RedirectToAction(nameof(Doctors));
    }

    // ── Medicines Management ─────────────────────────────────────────────────
    public async Task<IActionResult> Medicines(string? search = null)
    {
        var guard = RequireAdmin(); if (guard != null) return guard;
        var q = "api/medicines";
        if (!string.IsNullOrWhiteSpace(search)) q += $"?search={Uri.EscapeDataString(search)}";
        var result = await _api.GetAsync<List<MedicineDto>>(q);
        ViewBag.Search = search;
        return View(result?.Data ?? new List<MedicineDto>());
    }

    [HttpGet]
    public IActionResult AddMedicine()
    {
        var guard = RequireAdmin(); if (guard != null) return guard;
        return View(new CreateMedicineDto());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddMedicine(CreateMedicineDto dto)
    {
        var guard = RequireAdmin(); if (guard != null) return guard;
        if (!ModelState.IsValid) return View(dto);
        var result = await _api.PostAsync<MedicineDto>("api/medicines", dto);
        if (result?.Success == true)
        {
            TempData["Success"] = $"Medicine '{dto.Name}' added.";
            return RedirectToAction(nameof(Medicines));
        }
        ModelState.AddModelError(string.Empty, result?.Message ?? "Failed.");
        return View(dto);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteMedicine(int id)
    {
        var guard = RequireAdmin(); if (guard != null) return guard;
        await _api.DeleteAsync<bool>($"api/medicines/{id}");
        TempData["Success"] = "Medicine removed.";
        return RedirectToAction(nameof(Medicines));
    }

    // ── Specializations Management ───────────────────────────────────────────
    public async Task<IActionResult> Specializations()
    {
        var guard = RequireAdmin(); if (guard != null) return guard;
        var result = await _api.GetAsync<List<SpecializationDto>>("api/specializations");
        return View(result?.Data ?? new List<SpecializationDto>());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddSpecialization(string name, string? description)
    {
        var guard = RequireAdmin(); if (guard != null) return guard;
        if (string.IsNullOrWhiteSpace(name))
        {
            TempData["Error"] = "Name is required.";
            return RedirectToAction(nameof(Specializations));
        }
        var result = await _api.PostAsync<SpecializationDto>("api/specializations",
            new CreateSpecializationDto { Name = name.Trim(), Description = description?.Trim() });
        TempData[result?.Success == true ? "Success" : "Error"] =
            result?.Success == true ? $"Specialization '{name}' added." : result?.Message ?? "Failed.";
        return RedirectToAction(nameof(Specializations));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteSpecialization(int id)
    {
        var guard = RequireAdmin(); if (guard != null) return guard;
        await _api.DeleteAsync<bool>($"api/specializations/{id}");
        TempData["Success"] = "Specialization removed.";
        return RedirectToAction(nameof(Specializations));
    }

    // ── Reports ──────────────────────────────────────────────────────────────
    public async Task<IActionResult> Reports()
    {
        var guard = RequireAdmin(); if (guard != null) return guard;

        var patients     = await _api.GetAsync<PagedResult<PatientDto>>("api/patients?page=1&pageSize=1");
        var doctors      = await _api.GetAsync<PagedResult<DoctorDto>>("api/doctors?page=1&pageSize=1");
        var allAppts     = await _api.GetAsync<PagedResult<AppointmentDto>>("api/appointments?page=1&pageSize=1000");
        var todayAppts   = await _api.GetAsync<PagedResult<AppointmentDto>>(
            $"api/appointments?date={DateTime.Today:yyyy-MM-dd}&page=1&pageSize=1000");

        var apptList = allAppts?.Data?.Items ?? new();

        ViewBag.TotalPatients     = patients?.Data?.TotalCount ?? 0;
        ViewBag.TotalDoctors      = doctors?.Data?.TotalCount  ?? 0;
        ViewBag.TotalAppointments = allAppts?.Data?.TotalCount  ?? 0;
        ViewBag.TodayCount        = todayAppts?.Data?.TotalCount ?? 0;
        ViewBag.CompletedCount    = apptList.Count(a => a.Status == SmartHealthcare.Core.Enums.AppointmentStatus.Completed);
        ViewBag.CancelledCount    = apptList.Count(a => a.Status == SmartHealthcare.Core.Enums.AppointmentStatus.Cancelled);
        ViewBag.PendingCount      = apptList.Count(a => a.Status == SmartHealthcare.Core.Enums.AppointmentStatus.Pending);
        ViewBag.TotalRevenue      = apptList
            .Where(a => a.Status == SmartHealthcare.Core.Enums.AppointmentStatus.Completed && a.Fee.HasValue)
            .Sum(a => a.Fee!.Value);

        return View(apptList.OrderByDescending(a => a.AppointmentDate).Take(20).ToList());
    }

    // ── Departments Management ────────────────────────────────────────────────
    public async Task<IActionResult> Departments()
    {
        var guard = RequireAdmin(); if (guard != null) return guard;
        var result = await _api.GetAsync<List<DepartmentDto>>("api/departments");
        return View(result?.Data ?? new List<DepartmentDto>());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateDepartment(CreateDepartmentDto dto)
    {
        var guard = RequireAdmin(); if (guard != null) return guard;
        var result = await _api.PostAsync<DepartmentDto>("api/departments", dto);
        TempData[result?.Success == true ? "Success" : "Error"] =
            result?.Success == true ? $"Department '{dto.DepartmentName}' created." : result?.Message ?? "Failed.";
        return RedirectToAction(nameof(Departments));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteDepartment(int id)
    {
        var guard = RequireAdmin(); if (guard != null) return guard;
        await _api.DeleteAsync<bool>($"api/departments/{id}");
        TempData["Success"] = "Department removed.";
        return RedirectToAction(nameof(Departments));
    }

    // ── Bills Management ──────────────────────────────────────────────────────
    public async Task<IActionResult> Bills(string? paymentStatus = null)
    {
        var guard = RequireAdmin(); if (guard != null) return guard;
        var q = paymentStatus != null ? $"api/bills?paymentStatus={paymentStatus}" : "api/bills";
        var result = await _api.GetAsync<List<BillDto>>(q);
        ViewBag.FilterStatus = paymentStatus;
        return View(result?.Data ?? new List<BillDto>());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkBillPaid(int billId)
    {
        var guard = RequireAdmin(); if (guard != null) return guard;
        var result = await _api.PatchAsync<BillDto>($"api/bills/{billId}/payment",
            new { PaymentStatus = "Paid" });
        TempData[result?.Success == true ? "Success" : "Error"] =
            result?.Success == true ? "Bill marked as paid." : result?.Message ?? "Failed.";
        return RedirectToAction(nameof(Bills));
    }

}
