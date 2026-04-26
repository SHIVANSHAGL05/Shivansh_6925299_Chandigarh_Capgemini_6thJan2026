using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SmartHealthcare.Core.DTOs;
using SmartHealthcare.Core.Interfaces;

namespace SmartHealthcare.API.Controllers;

[Route("api/doctors")]
public class DoctorsController : BaseApiController
{
    private readonly IDoctorService _service;
    private readonly ISpecializationService _specService;
    private readonly IDepartmentService _deptService;
    private readonly ILogger<DoctorsController> _logger;

    public DoctorsController(
        IDoctorService service,
        ISpecializationService specService,
        IDepartmentService deptService,
        ILogger<DoctorsController> logger)
    {
        _service     = service;
        _specService = specService;
        _deptService = deptService;
        _logger      = logger;
    }

    /// <summary>GET all doctors with pagination, search, and optional specialization filter</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] int? specializationId = null,
        [FromQuery] int? departmentId = null)
    {
        // If departmentId is specified, delegate to department-scoped query
        if (departmentId.HasValue)
        {
            var deptResult = await _deptService.GetDoctorsByDepartmentAsync(departmentId.Value);
            return HandleResponse(deptResult);
        }
        var result = await _service.GetAllAsync(page, pageSize, search, specializationId);
        return HandleResponse(result);
    }

    /// <summary>GET doctor by ID</summary>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return HandleResponse(result);
    }

    /// <summary>GET current doctor's own profile</summary>
    [HttpGet("me")]
    [Authorize(Roles = "Doctor")]
    public async Task<IActionResult> GetMyProfile()
    {
        var result = await _service.GetByUserIdAsync(GetCurrentUserId());
        return HandleResponse(result);
    }

    /// <summary>GET doctors by specialization</summary>
    [HttpGet("by-specialization/{specializationId:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetBySpecialization(int specializationId)
    {
        var result = await _service.GetBySpecializationAsync(specializationId);
        return HandleResponse(result);
    }

    /// <summary>POST create/complete doctor profile</summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<IActionResult> Create([FromBody] CreateDoctorDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _service.CreateAsync(GetCurrentUserId(), dto);
        _logger.LogInformation("Doctor profile created for user {UserId}", GetCurrentUserId());
        return HandleResponse(result);
    }

    /// <summary>PUT full update of doctor profile</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDoctorDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _service.UpdateAsync(id, dto);
        return HandleResponse(result);
    }

    /// <summary>PATCH partial update (availability, fee, etc.)</summary>
    [HttpPatch("{id:int}")]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<IActionResult> Patch(int id, [FromBody] JsonPatchDocument<UpdateDoctorDto> patchDoc)
    {
        var existing = await _service.GetByIdAsync(id);
        if (!existing.Success) return NotFound(existing);

        var dto = new UpdateDoctorDto
        {
            LicenseNumber   = existing.Data!.LicenseNumber,
            Qualifications  = existing.Data.Qualifications,
            ExperienceYears = existing.Data.ExperienceYears,
            ConsultationFee = existing.Data.ConsultationFee,
            Biography       = existing.Data.Biography,
            IsAvailable     = existing.Data.IsAvailable,
            SpecializationIds = existing.Data.Specializations.Select(s => s.Id).ToList()
        };

        patchDoc.ApplyTo(dto, ModelState);
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _service.UpdateAsync(id, dto);
        return HandleResponse(result);
    }

    /// <summary>DELETE soft-delete doctor (Admin only)</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        _logger.LogWarning("Doctor {Id} deleted by admin {UserId}", id, GetCurrentUserId());
        return HandleResponse(result);
    }

    // ── Specializations sub-resource ────────────────────────────────────────

    /// <summary>GET all specializations (cached)</summary>
    [HttpGet("/api/specializations")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllSpecializations()
    {
        var result = await _specService.GetAllAsync();
        return HandleResponse(result);
    }

    /// <summary>POST create specialization (Admin only)</summary>
    [HttpPost("/api/specializations")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateSpecialization([FromBody] CreateSpecializationDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _specService.CreateAsync(dto);
        return HandleResponse(result);
    }

    /// <summary>DELETE specialization (Admin only)</summary>
    [HttpDelete("/api/specializations/{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteSpecialization(int id)
    {
        var result = await _specService.DeleteAsync(id);
        return HandleResponse(result);
    }
}
