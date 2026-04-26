using Microsoft.AspNetCore.Mvc;
using SmartHealthcare.MVC.Services;
using SmartHealthcare.Models.DTOs;

namespace SmartHealthcare.MVC.Controllers;

public class DoctorController : Controller
{
    private readonly IApiService _apiService;

    public DoctorController(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> Index(string? specialization, string? department, string? name, int page = 1)
    {
        var token = HttpContext.Session.GetString("Token");
        var endpoint = $"doctors?pageNumber={page}&pageSize=20";

        if (!string.IsNullOrWhiteSpace(specialization))
        {
            endpoint = $"doctors/search?specialization={Uri.EscapeDataString(specialization)}&pageNumber={page}&pageSize=20";
        }
        else if (!string.IsNullOrWhiteSpace(department))
        {
            endpoint = $"doctors/search-by-department?department={Uri.EscapeDataString(department)}&pageNumber={page}&pageSize=20";
        }

        var result = await _apiService.GetAsync<PagedResult<DoctorDTO>>(endpoint, token);
        ViewBag.Specialization = specialization;
        ViewBag.Department = department;
        ViewBag.Name = name;
        return View(result ?? new PagedResult<DoctorDTO>());
    }

    public async Task<IActionResult> Details(int id)
    {
        var token = HttpContext.Session.GetString("Token");
        var doctor = await _apiService.GetAsync<DoctorDTO>($"doctors/{id}", token);
        if (doctor == null)
        {
            return NotFound();
        }

        return View(doctor);
    }
}