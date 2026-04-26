using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHealthcare.Core.DTOs;
using SmartHealthcare.Core.Enums;
using SmartHealthcare.Core.Interfaces;

namespace SmartHealthcare.API.Controllers;

[Route("api/appointments")]
[Authorize]
public class AppointmentsController : BaseApiController
{
    private readonly IAppointmentService _service;
    private readonly ILogger<AppointmentsController> _logger;

    public AppointmentsController(IAppointmentService service, ILogger<AppointmentsController> logger)
    {
        _service = service;
        _logger  = logger;
    }

    /// <summary>GET all appointments with filters (Admin/Doctor)</summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<IActionResult> GetAll([FromQuery] AppointmentFilterDto filter)
    {
        var result = await _service.GetAllAsync(filter);
        return HandleResponse(result);
    }

    /// <summary>GET appointment by ID</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return HandleResponse(result);
    }

    /// <summary>GET appointments by date query param (e.g. /api/appointments?date=2026-04-01)</summary>
    [HttpGet("by-date")]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<IActionResult> GetByDate([FromQuery] DateTime date)
    {
        var filter = new AppointmentFilterDto { Date = date, Page = 1, PageSize = 100 };
        var result = await _service.GetAllAsync(filter);
        return HandleResponse(result);
    }

    /// <summary>GET current patient's appointments</summary>
    [HttpGet("my")]
    [Authorize(Roles = "Patient")]
    public async Task<IActionResult> GetMyAppointments()
    {
        var filter = new AppointmentFilterDto { Page = 1, PageSize = 50 };
        // PatientProfileId resolved inside service by UserId claim
        var result = await _service.GetAllAsync(filter);
        return HandleResponse(result);
    }

    /// <summary>GET appointments for a specific patient profile</summary>
    [HttpGet("patient/{patientProfileId:int}")]
    [Authorize(Roles = "Admin,Doctor,Patient")]
    public async Task<IActionResult> GetByPatient(int patientProfileId)
    {
        var result = await _service.GetByPatientAsync(patientProfileId);
        return HandleResponse(result);
    }

    /// <summary>GET appointments for a specific doctor (optionally filtered by date)</summary>
    [HttpGet("doctor/{doctorProfileId:int}")]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<IActionResult> GetByDoctor(int doctorProfileId, [FromQuery] DateTime? date = null)
    {
        var result = await _service.GetByDoctorAsync(doctorProfileId, date);
        return HandleResponse(result);
    }

    /// <summary>POST book a new appointment (Patient only)</summary>
    [HttpPost]
    [Authorize(Roles = "Patient")]
    public async Task<IActionResult> Book([FromBody] CreateAppointmentDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        _logger.LogInformation("Appointment booking by user {UserId} with doctor {DoctorId}",
            GetCurrentUserId(), dto.DoctorProfileId);

        var result = await _service.CreateAsync(GetCurrentUserId(), dto);

        if (result.Success)
            _logger.LogInformation("Appointment {Id} booked successfully", result.Data?.Id);

        return HandleResponse(result);
    }

    /// <summary>PUT full update of appointment (Patient, before confirmation)</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Patient")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAppointmentDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _service.UpdateAsync(id, dto, GetCurrentUserId());
        return HandleResponse(result);
    }

    /// <summary>PATCH update appointment status (Doctor only)</summary>
    [HttpPatch("{id:int}/status")]
    [Authorize(Roles = "Doctor,Admin")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateAppointmentStatusDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _service.UpdateStatusAsync(id, dto, GetCurrentUserId());
        _logger.LogInformation("Appointment {Id} status changed to {Status} by doctor {UserId}",
            id, dto.Status, GetCurrentUserId());
        return HandleResponse(result);
    }

    /// <summary>DELETE cancel appointment (Patient only)</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Patient,Admin")]
    public async Task<IActionResult> Cancel(int id)
    {
        var result = await _service.CancelAsync(id, GetCurrentUserId());
        _logger.LogInformation("Appointment {Id} cancelled by user {UserId}", id, GetCurrentUserId());
        return HandleResponse(result);
    }
}
