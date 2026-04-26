using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SmartHealthcare.Core.DTOs;
using SmartHealthcare.Core.Interfaces;

namespace SmartHealthcare.API.Controllers;

[Route("api/patients")]
[Authorize]
public class PatientsController : BaseApiController
{
    private readonly IPatientService _service;
    private readonly ILogger<PatientsController> _logger;

    public PatientsController(IPatientService service, ILogger<PatientsController> logger)
    {
        _service = service;
        _logger  = logger;
    }

    /// <summary>GET all patients (Admin only) with pagination and search</summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null)
    {
        var result = await _service.GetAllAsync(page, pageSize, search);
        return HandleResponse(result);
    }

    /// <summary>GET patient by ID</summary>
    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return HandleResponse(result);
    }

    /// <summary>GET current patient's own profile</summary>
    [HttpGet("me")]
    [Authorize(Roles = "Patient")]
    public async Task<IActionResult> GetMyProfile()
    {
        var result = await _service.GetByUserIdAsync(GetCurrentUserId());
        return HandleResponse(result);
    }

    /// <summary>POST create patient profile (usually auto-created on register)</summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Patient")]
    public async Task<IActionResult> Create([FromBody] CreatePatientDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _service.CreateAsync(GetCurrentUserId(), dto);
        _logger.LogInformation("Patient profile created for user {UserId}", GetCurrentUserId());
        return HandleResponse(result);
    }

    /// <summary>PUT full update of patient profile</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Patient")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePatientDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _service.UpdateAsync(id, dto);
        return HandleResponse(result);
    }

    /// <summary>PATCH partial update of patient profile</summary>
    [HttpPatch("{id:int}")]
    [Authorize(Roles = "Admin,Patient")]
    public async Task<IActionResult> Patch(int id, [FromBody] JsonPatchDocument<UpdatePatientDto> patchDoc)
    {
        var existing = await _service.GetByIdAsync(id);
        if (!existing.Success) return NotFound(existing);

        var dto = new UpdatePatientDto
        {
            BloodGroup            = existing.Data!.BloodGroup,
            AllergiesNotes        = existing.Data.AllergiesNotes,
            MedicalHistory        = existing.Data.MedicalHistory,
            EmergencyContactName  = existing.Data.EmergencyContactName,
            EmergencyContactPhone = existing.Data.EmergencyContactPhone,
            Weight = existing.Data.Weight,
            Height = existing.Data.Height
        };

        patchDoc.ApplyTo(dto, ModelState);
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _service.UpdateAsync(id, dto);
        return HandleResponse(result);
    }

    /// <summary>DELETE soft-delete patient (Admin only)</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        _logger.LogWarning("Patient {Id} deleted by {UserId}", id, GetCurrentUserId());
        return HandleResponse(result);
    }
}
