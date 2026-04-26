using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHealthcare.API.Services.Interfaces;
using SmartHealthcare.Models.DTOs;

namespace SmartHealthcare.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class BillsController : ControllerBase
{
    private readonly IBillService _service;

    public BillsController(IBillService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _service.GetAllAsync(pageNumber, pageSize);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetById(int id)
    {
        var bill = await _service.GetByIdAsync(id);
        if (bill == null)
        {
            return NotFound(new ErrorResponseDTO { Message = "Bill not found", StatusCode = 404 });
        }

        return Ok(bill);
    }

    [HttpGet("appointment/{appointmentId:int}")]
    public async Task<IActionResult> GetByAppointmentId(int appointmentId)
    {
        var bill = await _service.GetByAppointmentIdAsync(appointmentId);
        if (bill == null)
        {
            return NotFound(new ErrorResponseDTO { Message = "Bill not found for this appointment", StatusCode = 404 });
        }

        return Ok(bill);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateBillDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateBillDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var success = await _service.UpdateAsync(id, dto);
        if (!success)
        {
            return NotFound(new ErrorResponseDTO { Message = "Bill not found", StatusCode = 404 });
        }

        return Ok(new { message = "Bill updated successfully" });
    }

    [HttpPost("{id:int}/pay")]
    [Authorize(Roles = "Patient,Admin")]
    public async Task<IActionResult> Pay(int id, [FromBody] PayBillDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var success = await _service.MarkAsPaidAsync(id, dto);
        if (!success)
        {
            return NotFound(new ErrorResponseDTO { Message = "Bill not found", StatusCode = 404 });
        }

        return Ok(new { message = "Payment successful" });
    }
}
