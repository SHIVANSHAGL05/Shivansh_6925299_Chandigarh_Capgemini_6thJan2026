using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHealthcare.Core.DTOs;
using SmartHealthcare.Core.Interfaces;

namespace SmartHealthcare.API.Controllers;

[Route("api/prescriptions")]
[Authorize]
public class PrescriptionsController : BaseApiController
{
    private readonly IPrescriptionService _service;
    private readonly ILogger<PrescriptionsController> _logger;

    public PrescriptionsController(IPrescriptionService service, ILogger<PrescriptionsController> logger)
    {
        _service = service;
        _logger  = logger;
    }

    /// <summary>GET prescription by ID</summary>
    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,Doctor,Patient")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return HandleResponse(result);
    }

    /// <summary>GET prescription for a specific appointment</summary>
    [HttpGet("appointment/{appointmentId:int}")]
    [Authorize(Roles = "Admin,Doctor,Patient")]
    public async Task<IActionResult> GetByAppointment(int appointmentId)
    {
        var result = await _service.GetByAppointmentIdAsync(appointmentId);
        return HandleResponse(result);
    }

    /// <summary>GET all prescriptions for a patient</summary>
    [HttpGet("patient/{patientProfileId:int}")]
    [Authorize(Roles = "Admin,Doctor,Patient")]
    public async Task<IActionResult> GetByPatient(int patientProfileId)
    {
        var result = await _service.GetByPatientAsync(patientProfileId);
        return HandleResponse(result);
    }

    /// <summary>POST create a prescription (Doctor only)</summary>
    [HttpPost]
    [Authorize(Roles = "Doctor")]
    public async Task<IActionResult> Create([FromBody] CreatePrescriptionDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        _logger.LogInformation("Prescription created by doctor {UserId} for appointment {AppointmentId}",
            GetCurrentUserId(), dto.AppointmentId);

        var result = await _service.CreateAsync(dto, GetCurrentUserId());
        return HandleResponse(result);
    }
}
