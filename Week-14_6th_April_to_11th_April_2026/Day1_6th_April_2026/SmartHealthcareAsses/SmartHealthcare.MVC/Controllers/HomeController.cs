using Microsoft.AspNetCore.Mvc;
using SmartHealthcare.Core.Common;
using SmartHealthcare.Core.DTOs;
using SmartHealthcare.MVC.Models;
using SmartHealthcare.MVC.Services;

namespace SmartHealthcare.MVC.Controllers;

public class HomeController : Controller
{
    private readonly IApiService _api;
    private readonly ILogger<HomeController> _logger;

    public HomeController(IApiService api, ILogger<HomeController> logger)
    {
        _api    = api;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var token = HttpContext.Session.GetString("JwtToken");
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Account");

        var role = HttpContext.Session.GetString("UserRole");

        return role switch
        {
            "Admin"   => await AdminDashboard(),
            "Doctor"  => await DoctorDashboard(),
            "Patient" => await PatientDashboard(),
            _         => RedirectToAction("Login", "Account")
        };
    }

    private async Task<IActionResult> AdminDashboard()
    {
        var patients     = await _api.GetAsync<PagedResult<PatientDto>>("api/patients?page=1&pageSize=1");
        var doctors      = await _api.GetAsync<PagedResult<DoctorDto>>("api/doctors?page=1&pageSize=1");
        var appointments = await _api.GetAsync<PagedResult<AppointmentDto>>(
            $"api/appointments?page=1&pageSize=5");
        var todayAppts   = await _api.GetAsync<PagedResult<AppointmentDto>>(
            $"api/appointments?date={DateTime.Today:yyyy-MM-dd}&page=1&pageSize=100");

        var vm = new AdminDashboardViewModel
        {
            TotalPatients     = patients?.Data?.TotalCount ?? 0,
            TotalDoctors      = doctors?.Data?.TotalCount ?? 0,
            TotalAppointments = appointments?.Data?.TotalCount ?? 0,
            TodayAppointments = todayAppts?.Data?.TotalCount ?? 0,
            RecentAppointments = appointments?.Data?.Items ?? new()
        };
        return View("AdminDashboard", vm);
    }

    private async Task<IActionResult> DoctorDashboard()
    {
        var profile = await _api.GetAsync<DoctorDto>("api/doctors/me");
        if (profile?.Data == null) return RedirectToAction("Login", "Account");

        var todayAppts    = await _api.GetAsync<List<AppointmentDto>>(
            $"api/appointments/doctor/{profile.Data.Id}?date={DateTime.Today:yyyy-MM-dd}");
        var upcomingAppts = await _api.GetAsync<List<AppointmentDto>>(
            $"api/appointments/doctor/{profile.Data.Id}");

        var vm = new DoctorDashboardViewModel
        {
            Profile              = profile.Data,
            TodayAppointments    = todayAppts?.Data ?? new(),
            UpcomingAppointments = upcomingAppts?.Data?
                .Where(a => a.AppointmentDate > DateTime.Today).Take(10).ToList() ?? new()
        };
        return View("DoctorDashboard", vm);
    }

    private async Task<IActionResult> PatientDashboard()
    {
        var profile = await _api.GetAsync<PatientDto>("api/patients/me");
        if (profile?.Data == null) return RedirectToAction("Login", "Account");

        var allAppts = await _api.GetAsync<List<AppointmentDto>>(
            $"api/appointments/patient/{profile.Data.Id}");

        var now = DateTime.Now;
        var vm = new PatientDashboardViewModel
        {
            Profile              = profile.Data,
            UpcomingAppointments = allAppts?.Data?
                .Where(a => a.AppointmentDate >= now).Take(5).ToList() ?? new(),
            PastAppointments     = allAppts?.Data?
                .Where(a => a.AppointmentDate < now).Take(5).ToList() ?? new()
        };
        return View("PatientDashboard", vm);
    }

    public IActionResult Error() => View();
}
