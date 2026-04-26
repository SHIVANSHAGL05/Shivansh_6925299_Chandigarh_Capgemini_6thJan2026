using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SmartHealthcare.Core.Common;
using SmartHealthcare.Core.DTOs;
using SmartHealthcare.Core.Entities;
using SmartHealthcare.Core.Interfaces;
using SmartHealthcare.Infrastructure.Data;

namespace SmartHealthcare.Infrastructure.Services;

public class DoctorService : IDoctorService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;

    private const string AllDoctorsCacheKey      = "all_doctors";
    private const string SpecializationsCacheKey = "all_specializations";

    public DoctorService(AppDbContext context, IMapper mapper, IMemoryCache cache)
    {
        _context = context;
        _mapper  = mapper;
        _cache   = cache;
    }

    public async Task<ApiResponse<DoctorDto>> GetByIdAsync(int id)
    {
        var doctor = await IncludeDoctorQuery().FirstOrDefaultAsync(d => d.Id == id);
        return doctor is null
            ? ApiResponse<DoctorDto>.NotFound("Doctor not found.")
            : ApiResponse<DoctorDto>.Ok(_mapper.Map<DoctorDto>(doctor));
    }

    public async Task<ApiResponse<DoctorDto>> GetByUserIdAsync(int userId)
    {
        var doctor = await IncludeDoctorQuery().FirstOrDefaultAsync(d => d.UserId == userId);
        return doctor is null
            ? ApiResponse<DoctorDto>.NotFound("Doctor profile not found.")
            : ApiResponse<DoctorDto>.Ok(_mapper.Map<DoctorDto>(doctor));
    }

    public async Task<ApiResponse<PagedResult<DoctorDto>>> GetAllAsync(
        int page, int pageSize, string? search = null, int? specializationId = null)
    {
        // ── Cache the full unfiltered list (page 1, default size, no filters) ──
        bool isDefaultQuery = string.IsNullOrWhiteSpace(search)
                              && specializationId is null
                              && page == 1
                              && pageSize == 10;

        if (isDefaultQuery && _cache.TryGetValue(AllDoctorsCacheKey, out PagedResult<DoctorDto>? cached))
            return ApiResponse<PagedResult<DoctorDto>>.Ok(cached!);

        // ── Build query ───────────────────────────────────────────────────────
        var query = IncludeDoctorQuery();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(d =>
                d.User.FirstName.Contains(search) ||
                d.User.LastName.Contains(search)  ||
                d.LicenseNumber.Contains(search));

        if (specializationId.HasValue)
            query = query.Where(d =>
                d.DoctorSpecializations.Any(ds => ds.SpecializationId == specializationId));

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(d => d.User.LastName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var result = new PagedResult<DoctorDto>
        {
            Items      = _mapper.Map<List<DoctorDto>>(items),
            TotalCount = total,
            Page       = page,
            PageSize   = pageSize
        };

        // Cache only the default (unfiltered first page) result — 15-min expiry
        if (isDefaultQuery)
            _cache.Set(AllDoctorsCacheKey, result, TimeSpan.FromMinutes(15));

        return ApiResponse<PagedResult<DoctorDto>>.Ok(result);
    }

    public async Task<ApiResponse<DoctorDto>> CreateAsync(int userId, CreateDoctorDto dto)
    {
        if (await _context.DoctorProfiles.AnyAsync(d => d.UserId == userId))
            return ApiResponse<DoctorDto>.Fail("Doctor profile already exists.");

        var doctor = _mapper.Map<DoctorProfile>(dto);
        doctor.UserId = userId;

        foreach (var specId in dto.SpecializationIds.Distinct())
        {
            if (await _context.Specializations.AnyAsync(s => s.Id == specId))
                doctor.DoctorSpecializations.Add(new DoctorSpecializationMapping
                {
                    SpecializationId = specId,
                    IsPrimary        = specId == dto.SpecializationIds.FirstOrDefault()
                });
        }

        _context.DoctorProfiles.Add(doctor);
        await _context.SaveChangesAsync();
        InvalidateDoctorCache();
        return await GetByIdAsync(doctor.Id);
    }

    public async Task<ApiResponse<DoctorDto>> UpdateAsync(int id, UpdateDoctorDto dto)
    {
        var doctor = await _context.DoctorProfiles
            .Include(d => d.DoctorSpecializations)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (doctor is null)
            return ApiResponse<DoctorDto>.NotFound("Doctor not found.");

        _mapper.Map(dto, doctor);

        doctor.DoctorSpecializations.Clear();
        foreach (var specId in dto.SpecializationIds.Distinct())
        {
            if (await _context.Specializations.AnyAsync(s => s.Id == specId))
                doctor.DoctorSpecializations.Add(new DoctorSpecializationMapping
                {
                    SpecializationId = specId,
                    IsPrimary        = specId == dto.SpecializationIds.FirstOrDefault()
                });
        }

        await _context.SaveChangesAsync();
        InvalidateDoctorCache();
        return await GetByIdAsync(id);
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id)
    {
        var doctor = await _context.DoctorProfiles.FindAsync(id);
        if (doctor is null) return ApiResponse<bool>.NotFound("Doctor not found.");

        doctor.IsDeleted = true;
        await _context.SaveChangesAsync();
        InvalidateDoctorCache();
        return ApiResponse<bool>.Ok(true, "Doctor deleted.");
    }

    public async Task<ApiResponse<List<DoctorDto>>> GetBySpecializationAsync(int specializationId)
    {
        var cacheKey = $"doctors_spec_{specializationId}";
        if (!_cache.TryGetValue(cacheKey, out List<DoctorDto>? cached))
        {
            var doctors = await IncludeDoctorQuery()
                .Where(d => d.DoctorSpecializations.Any(ds => ds.SpecializationId == specializationId))
                .ToListAsync();

            cached = _mapper.Map<List<DoctorDto>>(doctors);
            _cache.Set(cacheKey, cached, TimeSpan.FromMinutes(10));
        }
        return ApiResponse<List<DoctorDto>>.Ok(cached!);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private void InvalidateDoctorCache()
    {
        _cache.Remove(AllDoctorsCacheKey);
        // Specialization-keyed caches are keyed by id and naturally expire
    }

    private IQueryable<DoctorProfile> IncludeDoctorQuery() =>
        _context.DoctorProfiles
            .Include(d => d.User)
            .Include(d => d.Department)
            .Include(d => d.DoctorSpecializations)
                .ThenInclude(ds => ds.Specialization)
            .AsQueryable();
}
