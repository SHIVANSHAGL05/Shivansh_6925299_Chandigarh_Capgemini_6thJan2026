using Microsoft.AspNetCore.Mvc;
using SmartHealthcare.Core.Common;
using SmartHealthcare.Core.DTOs;
using SmartHealthcare.Core.Enums;
using SmartHealthcare.MVC.Models;
using SmartHealthcare.MVC.Services;

namespace SmartHealthcare.MVC.Controllers;

public class AppointmentController : Controller
{
    private readonly IApiService _api;
    private readonly ILogger<AppointmentController> _logger;

    public AppointmentController(IApiService api, ILogger<AppointmentController> logger)
    {
        _api    = api;
        _logger = logger;
    }

    private bool IsAuthenticated => HttpContext.Session.GetString("JwtToken") != null;
    private string? UserRole     => HttpContext.Session.GetString("UserRole");

    // ── List ────────────────────────────────────────────────────────────────
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10,
        AppointmentStatus? status = null, DateTime? date = null)
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Account");

        string endpoint;
        if (UserRole == "Patient")
        {
            var profile = await _api.GetAsync<PatientDto>("api/patients/me");
            endpoint = $"api/appointments/patient/{profile?.Data?.Id}";
            var list = await _api.GetAsync<List<AppointmentDto>>(endpoint);
            var filtered = list?.Data ?? new();
            if (status.HasValue) filtered = filtered.Where(a => a.Status == status).ToList();
            return View(new AppointmentListViewModel
            {
                Appointments = filtered.Skip((page-1)*pageSize).Take(pageSize).ToList(),
                TotalCount   = filtered.Count,
                Page         = page, PageSize = pageSize
            });
        }
        else if (UserRole == "Doctor")
        {
            var profile = await _api.GetAsync<DoctorDto>("api/doctors/me");
            endpoint = $"api/appointments/doctor/{profile?.Data?.Id}";
            if (date.HasValue) endpoint += $"?date={date:yyyy-MM-dd}";
            var list = await _api.GetAsync<List<AppointmentDto>>(endpoint);
            var items = list?.Data ?? new();
            if (status.HasValue) items = items.Where(a => a.Status == status).ToList();
            return View(new AppointmentListViewModel
            {
                Appointments = items.Skip((page-1)*pageSize).Take(pageSize).ToList(),
                TotalCount   = items.Count, Page = page, PageSize = pageSize
            });
        }
        else // Admin
        {
            var q = $"api/appointments?page={page}&pageSize={pageSize}";
            if (status.HasValue) q += $"&status={status}";
            if (date.HasValue)   q += $"&date={date:yyyy-MM-dd}";
            var result = await _api.GetAsync<PagedResult<AppointmentDto>>(q);
            return View(new AppointmentListViewModel
            {
                Appointments = result?.Data?.Items ?? new(),
                TotalCount   = result?.Data?.TotalCount ?? 0,
                Page = page, PageSize = pageSize
            });
        }
    }

    // ── Detail ──────────────────────────────────────────────────────────────
    public async Task<IActionResult> Details(int id)
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Account");
        var result = await _api.GetAsync<AppointmentDto>($"api/appointments/{id}");
        if (result?.Data == null) return NotFound();
        return View(result.Data);
    }

    // ── Book ────────────────────────────────────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> Book(int? doctorId = null)
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Account");
        if (UserRole != "Patient") return Forbid();

        var doctors = await _api.GetAsync<PagedResult<DoctorDto>>("api/doctors?page=1&pageSize=100");
        var vm = new BookAppointmentViewModel
        {
            Doctors          = doctors?.Data?.Items ?? new(),
            DoctorProfileId  = doctorId ?? 0
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Book(BookAppointmentViewModel model)
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Account");

        if (!ModelState.IsValid)
        {
            var docs = await _api.GetAsync<PagedResult<DoctorDto>>("api/doctors?page=1&pageSize=100");
            model.Doctors = docs?.Data?.Items ?? new();
            return View(model);
        }

        if (!TimeSpan.TryParse(model.StartTimeStr, out var startTime))
        {
            ModelState.AddModelError("StartTimeStr", "Invalid time format.");
            var docs = await _api.GetAsync<PagedResult<DoctorDto>>("api/doctors?page=1&pageSize=100");
            model.Doctors = docs?.Data?.Items ?? new();
            return View(model);
        }

        var result = await _api.PostAsync<AppointmentDto>("api/appointments", new CreateAppointmentDto
        {
            DoctorProfileId = model.DoctorProfileId,
            AppointmentDate = model.AppointmentDate,
            StartTime       = startTime,
            ReasonForVisit  = model.ReasonForVisit,
            Type            = model.Type
        });

        if (result == null || !result.Success)
        {
            ModelState.AddModelError(string.Empty, result?.Message ?? "Booking failed.");
            var docs = await _api.GetAsync<PagedResult<DoctorDto>>("api/doctors?page=1&pageSize=100");
            model.Doctors = docs?.Data?.Items ?? new();
            return View(model);
        }

        _logger.LogInformation("Appointment booked: {Id}", result.Data?.Id);
        TempData["Success"] = "Appointment booked successfully!";
        return RedirectToAction(nameof(Index));
    }

    // ── Cancel ──────────────────────────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Account");

        var result = await _api.DeleteAsync<bool>($"api/appointments/{id}");
        TempData[result?.Success == true ? "Success" : "Error"] =
            result?.Success == true ? "Appointment cancelled." : result?.Message ?? "Cancellation failed.";

        return RedirectToAction(nameof(Index));
    }

    // ── Update Status (Doctor) ───────────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, AppointmentStatus status, string? notes)
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Account");

        var result = await _api.PatchAsync<AppointmentDto>($"api/appointments/{id}/status",
            new UpdateAppointmentStatusDto { Status = status, DoctorNotes = notes });

        TempData[result?.Success == true ? "Success" : "Error"] =
            result?.Success == true ? "Status updated." : result?.Message ?? "Update failed.";

        return RedirectToAction(nameof(Details), new { id });
    }
}
