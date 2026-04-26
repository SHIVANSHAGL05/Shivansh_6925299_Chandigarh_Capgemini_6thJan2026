using Microsoft.AspNetCore.Mvc;
using SmartHealthcare.Core.DTOs;
using SmartHealthcare.MVC.Services;

namespace SmartHealthcare.MVC.Controllers;

public class PrescriptionController : Controller
{
    private readonly IApiService _api;

    public PrescriptionController(IApiService api) => _api = api;

    private bool IsAuthenticated => HttpContext.Session.GetString("JwtToken") != null;

    public async Task<IActionResult> View(int appointmentId)
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Account");

        var result = await _api.GetAsync<PrescriptionDto>(
            $"api/prescriptions/appointment/{appointmentId}");

        if (result?.Data == null)
        {
            TempData["Error"] = "No prescription found for this appointment.";
            return RedirectToAction("Details", "Appointment", new { id = appointmentId });
        }
        return View(result.Data);
    }

    [HttpGet]
    public async Task<IActionResult> Create(int appointmentId)
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Account");
        if (HttpContext.Session.GetString("UserRole") != "Doctor")
            return RedirectToAction("AccessDenied", "Account");

        var medicines = await _api.GetAsync<List<MedicineDto>>("api/medicines");
        ViewBag.Medicines     = medicines?.Data ?? new List<MedicineDto>();
        ViewBag.AppointmentId = appointmentId;

        return View(new CreatePrescriptionDto { AppointmentId = appointmentId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreatePrescriptionDto dto)
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Account");

        // Remove empty medicine rows
        dto.Medicines = dto.Medicines
            .Where(m => m.MedicineId > 0 && !string.IsNullOrWhiteSpace(m.Dosage))
            .ToList();

        if (!ModelState.IsValid || !dto.Medicines.Any())
        {
            ModelState.AddModelError(string.Empty, "At least one medicine with dosage is required.");
            var meds = await _api.GetAsync<List<MedicineDto>>("api/medicines");
            ViewBag.Medicines     = meds?.Data ?? new List<MedicineDto>();
            ViewBag.AppointmentId = dto.AppointmentId;
            return View(dto);
        }

        var result = await _api.PostAsync<PrescriptionDto>("api/prescriptions", dto);

        if (result?.Success == true)
        {
            TempData["Success"] = "Prescription created successfully.";
            return RedirectToAction("Details", "Appointment", new { id = dto.AppointmentId });
        }

        ModelState.AddModelError(string.Empty, result?.Message ?? "Failed to create prescription.");
        var medicines = await _api.GetAsync<List<MedicineDto>>("api/medicines");
        ViewBag.Medicines     = medicines?.Data ?? new List<MedicineDto>();
        ViewBag.AppointmentId = dto.AppointmentId;
        return View(dto);
    }
}
