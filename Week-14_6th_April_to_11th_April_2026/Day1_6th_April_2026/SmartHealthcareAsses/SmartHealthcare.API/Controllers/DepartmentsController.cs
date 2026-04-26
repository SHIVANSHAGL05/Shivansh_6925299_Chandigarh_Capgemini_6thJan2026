using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHealthcare.Core.DTOs;
using SmartHealthcare.Core.Interfaces;

namespace SmartHealthcare.API.Controllers;

[Route("api/departments")]
public class DepartmentsController : BaseApiController
{
    private readonly IDepartmentService _service;
    private readonly ILogger<DepartmentsController> _logger;

    public DepartmentsController(IDepartmentService service, ILogger<DepartmentsController> logger)
    {
        _service = service;
        _logger  = logger;
    }

    /// <summary>GET all departments (public — used in search/filter dropdowns)</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return HandleResponse(result);
    }

    /// <summary>GET department by ID</summary>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return HandleResponse(result);
    }

    /// <summary>GET all doctors in a department — supports "search doctors by department"</summary>
    [HttpGet("{id:int}/doctors")]
    [AllowAnonymous]
    public async Task<IActionResult> GetDoctors(int id)
    {
        var result = await _service.GetDoctorsByDepartmentAsync(id);
        return HandleResponse(result);
    }

    /// <summary>POST create department (Admin only)</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateDepartmentDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        _logger.LogInformation("Admin creating department: {Name}", dto.DepartmentName);
        var result = await _service.CreateAsync(dto);
        return HandleResponse(result);
    }

    /// <summary>PUT update department (Admin only)</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDepartmentDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _service.UpdateAsync(id, dto);
        return HandleResponse(result);
    }

    /// <summary>DELETE department (Admin only)</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        _logger.LogInformation("Admin deleted department {Id}", id);
        return HandleResponse(result);
    }
}
