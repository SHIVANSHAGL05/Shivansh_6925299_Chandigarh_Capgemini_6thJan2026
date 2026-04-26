using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHealthcare.API.Services.Interfaces;
using SmartHealthcare.Models.DTOs;

namespace SmartHealthcare.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PrescriptionsController : ControllerBase
{
	private readonly IPrescriptionService _service;

	public PrescriptionsController(IPrescriptionService service) => _service = service;

	[HttpGet("appointment/{appointmentId:int}")]
	public async Task<IActionResult> GetByAppointmentId(int appointmentId)
	{
		var prescription = await _service.GetByAppointmentIdAsync(appointmentId);
		if (prescription == null) return NotFound(new ErrorResponseDTO { Message = "Prescription not found", StatusCode = 404 });
		return Ok(prescription);
	}

	[HttpPost]
	[Authorize(Roles = "Doctor")]
	public async Task<IActionResult> Create([FromBody] CreatePrescriptionDTO dto)
	{
		if (!ModelState.IsValid) return BadRequest(ModelState);
		var result = await _service.CreateAsync(dto);
		return CreatedAtAction(nameof(GetByAppointmentId), new { appointmentId = result.AppointmentId }, result);
	}
}
