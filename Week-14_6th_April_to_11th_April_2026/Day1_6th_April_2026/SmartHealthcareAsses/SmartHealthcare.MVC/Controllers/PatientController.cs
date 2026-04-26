using Microsoft.AspNetCore.Mvc;
using SmartHealthcare.Core.DTOs;
using SmartHealthcare.MVC.Services;

namespace SmartHealthcare.MVC.Controllers;

public class PatientController : Controller
{
    private readonly IApiService _api;

    public PatientController(IApiService api) => _api = api;

    private bool IsAuthenticated => HttpContext.Session.GetString("JwtToken") != null;

    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Account");

        var result = await _api.GetAsync<PatientDto>("api/patients/me");
        if (result?.Data == null)
            return View("NoProfile");

        return View(result.Data);
    }

    [HttpGet]
    public async Task<IActionResult> EditProfile()
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Account");

        var result = await _api.GetAsync<PatientDto>("api/patients/me");
        if (result?.Data == null) return View("NoProfile");

        var dto = new UpdatePatientDto
        {
            BloodGroup            = result.Data.BloodGroup,
            AllergiesNotes        = result.Data.AllergiesNotes,
            MedicalHistory        = result.Data.MedicalHistory,
            EmergencyContactName  = result.Data.EmergencyContactName,
            EmergencyContactPhone = result.Data.EmergencyContactPhone,
            Weight                = result.Data.Weight,
            Height                = result.Data.Height
        };
        ViewBag.PatientId = result.Data.Id;
        return View(dto);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProfile(int patientId, UpdatePatientDto dto)
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Account");

        if (!ModelState.IsValid)
        {
            ViewBag.PatientId = patientId;
            return View(dto);
        }

        var result = await _api.PutAsync<PatientDto>($"api/patients/{patientId}", dto);
        if (result?.Success == true)
        {
            TempData["Success"] = "Profile updated successfully.";
            return RedirectToAction(nameof(Profile));
        }

        ModelState.AddModelError(string.Empty, result?.Message ?? "Update failed.");
        ViewBag.PatientId = patientId;
        return View(dto);
    }

    public async Task<IActionResult> MyPrescriptions()
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Account");

        var profile = await _api.GetAsync<PatientDto>("api/patients/me");
        if (profile?.Data == null) return View("NoProfile");

        var prescriptions = await _api.GetAsync<List<PrescriptionDto>>(
            $"api/prescriptions/patient/{profile.Data.Id}");

        return View(prescriptions?.Data ?? new List<PrescriptionDto>());
    }

    public async Task<IActionResult> MyBills()
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Account");

        var profile = await _api.GetAsync<PatientDto>("api/patients/me");
        if (profile?.Data == null) return View("NoProfile");

        // Fetch all bills and filter by patient's appointments
        var appts = await _api.GetAsync<List<AppointmentDto>>(
            $"api/appointments/patient/{profile.Data.Id}");

        var bills = new List<BillDto>();
        foreach (var appt in appts?.Data ?? new())
        {
            var bill = await _api.GetAsync<BillDto>($"api/bills/appointment/{appt.Id}");
            if (bill?.Data != null) bills.Add(bill.Data);
        }
        return View(bills.OrderByDescending(b => b.AppointmentDate).ToList());
    }

}
