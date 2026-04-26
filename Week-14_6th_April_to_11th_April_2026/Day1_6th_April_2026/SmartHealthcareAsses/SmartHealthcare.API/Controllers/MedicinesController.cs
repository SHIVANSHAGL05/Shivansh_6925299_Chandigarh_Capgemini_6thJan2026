using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHealthcare.Core.DTOs;
using SmartHealthcare.Core.Interfaces;

namespace SmartHealthcare.API.Controllers;

[Route("api/medicines")]
public class MedicinesController : BaseApiController
{
    private readonly IMedicineService _service;

    public MedicinesController(IMedicineService service) => _service = service;

    /// <summary>GET all medicines with optional search (cached)</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] string? search = null)
    {
        var result = await _service.GetAllAsync(search);
        return HandleResponse(result);
    }

    /// <summary>GET medicine by ID</summary>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return HandleResponse(result);
    }

    /// <summary>POST create medicine (Admin only)</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateMedicineDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _service.CreateAsync(dto);
        return HandleResponse(result);
    }

    /// <summary>PUT full update medicine (Admin only)</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateMedicineDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _service.UpdateAsync(id, dto);
        return HandleResponse(result);
    }

    /// <summary>DELETE soft-delete medicine (Admin only)</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        return HandleResponse(result);
    }
}
