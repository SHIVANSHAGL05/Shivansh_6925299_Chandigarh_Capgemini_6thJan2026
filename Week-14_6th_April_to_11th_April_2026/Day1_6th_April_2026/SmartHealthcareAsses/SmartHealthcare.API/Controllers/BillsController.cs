using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHealthcare.Core.DTOs;
using SmartHealthcare.Core.Interfaces;

namespace SmartHealthcare.API.Controllers;

[Route("api/bills")]
[Authorize]
public class BillsController : BaseApiController
{
    private readonly IBillService _service;
    private readonly ILogger<BillsController> _logger;

    public BillsController(IBillService service, ILogger<BillsController> logger)
    {
        _service = service;
        _logger  = logger;
    }

    /// <summary>GET all bills — optionally filtered by payment status (Admin only)</summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll([FromQuery] string? paymentStatus = null)
    {
        var result = await _service.GetAllAsync(paymentStatus);
        return HandleResponse(result);
    }

    /// <summary>GET bill by ID</summary>
    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,Doctor,Patient")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return HandleResponse(result);
    }

    /// <summary>GET bill for a specific appointment</summary>
    [HttpGet("appointment/{appointmentId:int}")]
    [Authorize(Roles = "Admin,Doctor,Patient")]
    public async Task<IActionResult> GetByAppointment(int appointmentId)
    {
        var result = await _service.GetByAppointmentIdAsync(appointmentId);
        return HandleResponse(result);
    }

    /// <summary>POST create bill for an appointment (Doctor/Admin)</summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<IActionResult> Create([FromBody] CreateBillDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        _logger.LogInformation("Creating bill for appointment {AppointmentId}", dto.AppointmentId);
        var result = await _service.CreateAsync(dto);
        return HandleResponse(result);
    }

    /// <summary>PATCH update payment status (Paid/Unpaid) — Admin only</summary>
    [HttpPatch("{id:int}/payment")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdatePayment(int id, [FromBody] UpdateBillPaymentDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        _logger.LogInformation("Bill {Id} payment status updated to {Status}", id, dto.PaymentStatus);
        var result = await _service.UpdatePaymentStatusAsync(id, dto);
        return HandleResponse(result);
    }
}
