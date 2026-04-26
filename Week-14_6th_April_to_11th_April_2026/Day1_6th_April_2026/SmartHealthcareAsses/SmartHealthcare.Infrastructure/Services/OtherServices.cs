using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SmartHealthcare.Core.Common;
using SmartHealthcare.Core.DTOs;
using SmartHealthcare.Core.Entities;
using SmartHealthcare.Core.Enums;
using SmartHealthcare.Core.Interfaces;
using SmartHealthcare.Infrastructure.Data;

namespace SmartHealthcare.Infrastructure.Services;

// ── Prescription Service ─────────────────────────────────────────────────────
public class PrescriptionService : IPrescriptionService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public PrescriptionService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper  = mapper;
    }

    public async Task<ApiResponse<PrescriptionDto>> GetByIdAsync(int id)
    {
        var p = await IncludeQuery().FirstOrDefaultAsync(x => x.Id == id);
        return p is null
            ? ApiResponse<PrescriptionDto>.NotFound()
            : ApiResponse<PrescriptionDto>.Ok(_mapper.Map<PrescriptionDto>(p));
    }

    public async Task<ApiResponse<PrescriptionDto>> GetByAppointmentIdAsync(int appointmentId)
    {
        var p = await IncludeQuery().FirstOrDefaultAsync(x => x.AppointmentId == appointmentId);
        return p is null
            ? ApiResponse<PrescriptionDto>.NotFound("No prescription for this appointment.")
            : ApiResponse<PrescriptionDto>.Ok(_mapper.Map<PrescriptionDto>(p));
    }

    public async Task<ApiResponse<List<PrescriptionDto>>> GetByPatientAsync(int patientProfileId)
    {
        var list = await IncludeQuery()
            .Where(p => p.Appointment.PatientProfileId == patientProfileId)
            .OrderByDescending(p => p.IssuedDate)
            .ToListAsync();
        return ApiResponse<List<PrescriptionDto>>.Ok(_mapper.Map<List<PrescriptionDto>>(list));
    }

    public async Task<ApiResponse<PrescriptionDto>> CreateAsync(CreatePrescriptionDto dto, int doctorUserId)
    {
        var appt = await _context.Appointments
            .Include(a => a.DoctorProfile)
            .FirstOrDefaultAsync(a => a.Id == dto.AppointmentId);

        if (appt is null)
            return ApiResponse<PrescriptionDto>.NotFound("Appointment not found.");

        if (appt.DoctorProfile.UserId != doctorUserId)
            return ApiResponse<PrescriptionDto>.Unauthorized("Only the assigned doctor can create a prescription.");

        if (appt.Status != AppointmentStatus.Completed && appt.Status != AppointmentStatus.InProgress)
            return ApiResponse<PrescriptionDto>.Fail("Prescription can only be created for in-progress or completed appointments.");

        if (await _context.Prescriptions.AnyAsync(p => p.AppointmentId == dto.AppointmentId))
            return ApiResponse<PrescriptionDto>.Fail("Prescription already exists for this appointment.");

        var prescription = _mapper.Map<Prescription>(dto);
        prescription.IssuedDate = DateTime.UtcNow;

        foreach (var med in dto.Medicines)
        {
            if (!await _context.Medicines.AnyAsync(m => m.Id == med.MedicineId))
                return ApiResponse<PrescriptionDto>.Fail($"Medicine ID {med.MedicineId} not found.");

            prescription.PrescriptionMedicines.Add(_mapper.Map<PrescriptionMedicine>(med));
        }

        _context.Prescriptions.Add(prescription);
        await _context.SaveChangesAsync();
        return await GetByIdAsync(prescription.Id);
    }

    private IQueryable<Prescription> IncludeQuery() =>
        _context.Prescriptions
            .Include(p => p.Appointment).ThenInclude(a => a.PatientProfile).ThenInclude(p => p.User)
            .Include(p => p.Appointment).ThenInclude(a => a.DoctorProfile).ThenInclude(d => d.User)
            .Include(p => p.PrescriptionMedicines).ThenInclude(pm => pm.Medicine)
            .AsQueryable();
}

// ── Medicine Service ─────────────────────────────────────────────────────────
public class MedicineService : IMedicineService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;
    private const string CacheKey = "all_medicines";

    public MedicineService(AppDbContext context, IMapper mapper, IMemoryCache cache)
    {
        _context = context;
        _mapper  = mapper;
        _cache   = cache;
    }

    public async Task<ApiResponse<List<MedicineDto>>> GetAllAsync(string? search = null)
    {
        if (string.IsNullOrWhiteSpace(search) && _cache.TryGetValue(CacheKey, out List<MedicineDto>? cached))
            return ApiResponse<List<MedicineDto>>.Ok(cached!);

        var query = _context.Medicines.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(m => m.Name.Contains(search) || (m.GenericName != null && m.GenericName.Contains(search)));

        var items = _mapper.Map<List<MedicineDto>>(await query.OrderBy(m => m.Name).ToListAsync());
        if (string.IsNullOrWhiteSpace(search))
            _cache.Set(CacheKey, items, TimeSpan.FromMinutes(30));

        return ApiResponse<List<MedicineDto>>.Ok(items);
    }

    public async Task<ApiResponse<MedicineDto>> GetByIdAsync(int id)
    {
        var m = await _context.Medicines.FindAsync(id);
        return m is null ? ApiResponse<MedicineDto>.NotFound() : ApiResponse<MedicineDto>.Ok(_mapper.Map<MedicineDto>(m));
    }

    public async Task<ApiResponse<MedicineDto>> CreateAsync(CreateMedicineDto dto)
    {
        var med = _mapper.Map<Medicine>(dto);
        _context.Medicines.Add(med);
        await _context.SaveChangesAsync();
        _cache.Remove(CacheKey);
        return ApiResponse<MedicineDto>.Created(_mapper.Map<MedicineDto>(med));
    }

    public async Task<ApiResponse<MedicineDto>> UpdateAsync(int id, CreateMedicineDto dto)
    {
        var med = await _context.Medicines.FindAsync(id);
        if (med is null) return ApiResponse<MedicineDto>.NotFound();
        _mapper.Map(dto, med);
        await _context.SaveChangesAsync();
        _cache.Remove(CacheKey);
        return ApiResponse<MedicineDto>.Ok(_mapper.Map<MedicineDto>(med));
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id)
    {
        var med = await _context.Medicines.FindAsync(id);
        if (med is null) return ApiResponse<bool>.NotFound();
        med.IsDeleted = true;
        await _context.SaveChangesAsync();
        _cache.Remove(CacheKey);
        return ApiResponse<bool>.Ok(true);
    }
}

// ── Specialization Service ───────────────────────────────────────────────────
public class SpecializationService : ISpecializationService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;
    private const string CacheKey = "all_specializations";

    public SpecializationService(AppDbContext context, IMapper mapper, IMemoryCache cache)
    {
        _context = context;
        _mapper  = mapper;
        _cache   = cache;
    }

    public async Task<ApiResponse<List<SpecializationDto>>> GetAllAsync()
    {
        if (!_cache.TryGetValue(CacheKey, out List<SpecializationDto>? cached))
        {
            var items = await _context.Specializations.OrderBy(s => s.Name).ToListAsync();
            cached = _mapper.Map<List<SpecializationDto>>(items);
            _cache.Set(CacheKey, cached, TimeSpan.FromHours(1));
        }
        return ApiResponse<List<SpecializationDto>>.Ok(cached!);
    }

    public async Task<ApiResponse<SpecializationDto>> GetByIdAsync(int id)
    {
        var s = await _context.Specializations.FindAsync(id);
        return s is null ? ApiResponse<SpecializationDto>.NotFound() : ApiResponse<SpecializationDto>.Ok(_mapper.Map<SpecializationDto>(s));
    }

    public async Task<ApiResponse<SpecializationDto>> CreateAsync(CreateSpecializationDto dto)
    {
        var spec = _mapper.Map<Specialization>(dto);
        _context.Specializations.Add(spec);
        await _context.SaveChangesAsync();
        _cache.Remove(CacheKey);
        return ApiResponse<SpecializationDto>.Created(_mapper.Map<SpecializationDto>(spec));
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id)
    {
        var spec = await _context.Specializations.FindAsync(id);
        if (spec is null) return ApiResponse<bool>.NotFound();
        spec.IsDeleted = true;
        await _context.SaveChangesAsync();
        _cache.Remove(CacheKey);
        return ApiResponse<bool>.Ok(true);
    }
}
