using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartHealthcare.Core.Common;
using SmartHealthcare.Core.DTOs;
using SmartHealthcare.Core.Entities;
using SmartHealthcare.Core.Interfaces;
using SmartHealthcare.Infrastructure.Data;

namespace SmartHealthcare.Infrastructure.Services;

public class PatientService : IPatientService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;

    public PatientService(AppDbContext context, IMapper mapper, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _mapper  = mapper;
        _userManager = userManager;
    }

    public async Task<ApiResponse<PatientDto>> GetByIdAsync(int id)
    {
        var patient = await _context.PatientProfiles
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == id);

        return patient is null
            ? ApiResponse<PatientDto>.NotFound("Patient not found.")
            : ApiResponse<PatientDto>.Ok(_mapper.Map<PatientDto>(patient));
    }

    public async Task<ApiResponse<PatientDto>> GetByUserIdAsync(int userId)
    {
        var patient = await _context.PatientProfiles
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.UserId == userId);

        return patient is null
            ? ApiResponse<PatientDto>.NotFound("Patient profile not found.")
            : ApiResponse<PatientDto>.Ok(_mapper.Map<PatientDto>(patient));
    }

    public async Task<ApiResponse<PagedResult<PatientDto>>> GetAllAsync(int page, int pageSize, string? search = null)
    {
        var query = _context.PatientProfiles
            .Include(p => p.User)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p =>
                p.User.FirstName.Contains(search) ||
                p.User.LastName.Contains(search)  ||
                p.User.Email!.Contains(search));

        var total = await query.CountAsync();
        var items = await query.OrderBy(p => p.User.LastName)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .ToListAsync();

        return ApiResponse<PagedResult<PatientDto>>.Ok(new PagedResult<PatientDto>
        {
            Items      = _mapper.Map<List<PatientDto>>(items),
            TotalCount = total,
            Page       = page,
            PageSize   = pageSize
        });
    }

    public async Task<ApiResponse<PatientDto>> CreateAsync(int userId, CreatePatientDto dto)
    {
        var existing = await _context.PatientProfiles.AnyAsync(p => p.UserId == userId);
        if (existing)
            return ApiResponse<PatientDto>.Fail("Patient profile already exists.");

        var patient = _mapper.Map<PatientProfile>(dto);
        patient.UserId = userId;

        _context.PatientProfiles.Add(patient);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(patient.Id);
    }

    public async Task<ApiResponse<PatientDto>> UpdateAsync(int id, UpdatePatientDto dto)
    {
        var patient = await _context.PatientProfiles.FindAsync(id);
        if (patient is null)
            return ApiResponse<PatientDto>.NotFound("Patient not found.");

        _mapper.Map(dto, patient);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id)
    {
        var patient = await _context.PatientProfiles.FindAsync(id);
        if (patient is null)
            return ApiResponse<bool>.NotFound("Patient not found.");

        patient.IsDeleted = true;
        await _context.SaveChangesAsync();
        return ApiResponse<bool>.Ok(true, "Patient deleted.");
    }
}
