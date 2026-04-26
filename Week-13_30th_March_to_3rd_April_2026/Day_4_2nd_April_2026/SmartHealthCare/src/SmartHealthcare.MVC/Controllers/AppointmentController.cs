using Microsoft.AspNetCore.Mvc;
using SmartHealthcare.MVC.Services;
using SmartHealthcare.Models.DTOs;

namespace SmartHealthcare.MVC.Controllers;

public class AppointmentController : Controller
{
    private readonly IApiService _apiService;
    private readonly ILogger<AppointmentController> _logger;

    public AppointmentController(IApiService apiService, ILogger<AppointmentController> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var token = HttpContext.Session.GetString("Token");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Account");
        }

        var role = HttpContext.Session.GetString("Role");
        PagedResult<AppointmentDTO>? result = null;

        if (role == "Patient")
        {
            result = await _apiService.GetAsync<PagedResult<AppointmentDTO>>("appointments/my-appointments?pageNumber=1&pageSize=50", token);
        }
        else if (role == "Doctor")
        {
            result = await _apiService.GetAsync<PagedResult<AppointmentDTO>>("appointments/doctor-appointments?pageNumber=1&pageSize=50", token);
        }
        else if (role == "Admin")
        {
            result = await _apiService.GetAsync<PagedResult<AppointmentDTO>>("appointments?pageNumber=1&pageSize=50", token);
        }

        ViewBag.Role = role;
        return View(result?.Items ?? Enumerable.Empty<AppointmentDTO>());
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var token = HttpContext.Session.GetString("Token");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Account");
        }

        var doctorsResult = await _apiService.GetAsync<PagedResult<DoctorDTO>>("doctors?pageNumber=1&pageSize=200", token);
        ViewBag.Doctors = doctorsResult?.Items ?? Enumerable.Empty<DoctorDTO>();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateAppointmentDTO dto)
    {
        var token = HttpContext.Session.GetString("Token");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Account");
        }

        if (!ModelState.IsValid)
        {
            var doctorsResult = await _apiService.GetAsync<PagedResult<DoctorDTO>>("doctors?pageNumber=1&pageSize=200", token);
            ViewBag.Doctors = doctorsResult?.Items ?? Enumerable.Empty<DoctorDTO>();
            return View(dto);
        }

        var result = await _apiService.PostAsync<AppointmentDTO>("appointments", dto, token);
        if (result == null)
        {
            ModelState.AddModelError(string.Empty, "Failed to create appointment. Please ensure your patient profile is set up.");
            var doctorsResult = await _apiService.GetAsync<PagedResult<DoctorDTO>>("doctors?pageNumber=1&pageSize=200", token);
            ViewBag.Doctors = doctorsResult?.Items ?? Enumerable.Empty<DoctorDTO>();
            return View(dto);
        }

        _logger.LogInformation("Appointment booked successfully");
        TempData["Success"] = "Appointment booked successfully!";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        var token = HttpContext.Session.GetString("Token");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Account");
        }

        var appointment = await _apiService.GetAsync<AppointmentDTO>($"appointments/{id}", token);
        if (appointment == null)
        {
            return NotFound();
        }

        ViewBag.Role = HttpContext.Session.GetString("Role");
        ViewBag.Prescription = await _apiService.GetAsync<PrescriptionDTO>($"prescriptions/appointment/{id}", token);
        ViewBag.Bill = await _apiService.GetAsync<BillDTO>($"bills/appointment/{id}", token);
        return View(appointment);
    }

    [HttpPost]
    public async Task<IActionResult> PayBill(int appointmentId, int billId, string? transactionReference)
    {
        var token = HttpContext.Session.GetString("Token");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Account");
        }

        var payload = new PayBillDTO { TransactionReference = transactionReference };
        var result = await _apiService.PostAsync<object>($"bills/{billId}/pay", payload, token);

        if (result == null)
        {
            TempData["Error"] = "Payment failed. Please try again.";
        }
        else
        {
            TempData["Success"] = "Payment completed successfully.";
        }

        return RedirectToAction(nameof(Details), new { id = appointmentId });
    }

    [HttpPost]
    public async Task<IActionResult> MarkCompleted(int appointmentId)
    {
        var token = HttpContext.Session.GetString("Token");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Account");
        }

        var role = HttpContext.Session.GetString("Role");
        if (role != "Doctor" && role != "Admin")
        {
            TempData["Error"] = "You are not authorized to complete this appointment.";
            return RedirectToAction(nameof(Details), new { id = appointmentId });
        }

        var patchData = new Dictionary<string, object>
        {
            ["Status"] = "Completed"
        };

        var result = await _apiService.PatchAsync<object>($"appointments/{appointmentId}", patchData, token);
        if (result == null)
        {
            TempData["Error"] = "Failed to mark appointment as completed.";
        }
        else
        {
            TempData["Success"] = "Appointment marked as completed.";
        }

        return RedirectToAction(nameof(Details), new { id = appointmentId });
    }
}