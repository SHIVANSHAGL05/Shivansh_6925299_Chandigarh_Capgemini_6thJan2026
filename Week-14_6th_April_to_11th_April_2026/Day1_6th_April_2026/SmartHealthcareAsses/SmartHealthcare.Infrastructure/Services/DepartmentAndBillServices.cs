using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SmartHealthcare.Core.Common;
using SmartHealthcare.Core.DTOs;
using SmartHealthcare.Core.Entities;
using SmartHealthcare.Core.Interfaces;
using SmartHealthcare.Infrastructure.Data;

namespace SmartHealthcare.Infrastructure.Services;

// ── Department Service ────────────────────────────────────────────────────────
public class DepartmentService : IDepartmentService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;
    private const string CacheKey = "all_departments";

    public DepartmentService(AppDbContext context, IMapper mapper, IMemoryCache cache)
    {
        _context = context;
        _mapper  = mapper;
        _cache   = cache;
    }

    public async Task<ApiResponse<List<DepartmentDto>>> GetAllAsync()
    {
        if (!_cache.TryGetValue(CacheKey, out List<DepartmentDto>? cached))
        {
            var items = await _context.Departments
                .Include(d => d.Doctors)
                .OrderBy(d => d.DepartmentName)
                .ToListAsync();
            cached = _mapper.Map<List<DepartmentDto>>(items);
            _cache.Set(CacheKey, cached, TimeSpan.FromHours(1));
        }
        return ApiResponse<List<DepartmentDto>>.Ok(cached!);
    }

    public async Task<ApiResponse<DepartmentDto>> GetByIdAsync(int id)
    {
        var dept = await _context.Departments
            .Include(d => d.Doctors)
            .FirstOrDefaultAsync(d => d.Id == id);
        return dept is null
            ? ApiResponse<DepartmentDto>.NotFound("Department not found.")
            : ApiResponse<DepartmentDto>.Ok(_mapper.Map<DepartmentDto>(dept));
    }

    public async Task<ApiResponse<List<DoctorDto>>> GetDoctorsByDepartmentAsync(int departmentId)
    {
        var doctors = await _context.DoctorProfiles
            .Include(d => d.User)
            .Include(d => d.Department)
            .Include(d => d.DoctorSpecializations).ThenInclude(ds => ds.Specialization)
            .Where(d => d.DepartmentId == departmentId)
            .ToListAsync();
        return ApiResponse<List<DoctorDto>>.Ok(_mapper.Map<List<DoctorDto>>(doctors));
    }

    public async Task<ApiResponse<DepartmentDto>> CreateAsync(CreateDepartmentDto dto)
    {
        if (await _context.Departments.AnyAsync(d => d.DepartmentName == dto.DepartmentName))
            return ApiResponse<DepartmentDto>.Fail("A department with this name already exists.");

        var dept = _mapper.Map<Department>(dto);
        _context.Departments.Add(dept);
        await _context.SaveChangesAsync();
        _cache.Remove(CacheKey);
        return ApiResponse<DepartmentDto>.Created(_mapper.Map<DepartmentDto>(dept));
    }

    public async Task<ApiResponse<DepartmentDto>> UpdateAsync(int id, UpdateDepartmentDto dto)
    {
        var dept = await _context.Departments.FindAsync(id);
        if (dept is null) return ApiResponse<DepartmentDto>.NotFound("Department not found.");

        _mapper.Map(dto, dept);
        await _context.SaveChangesAsync();
        _cache.Remove(CacheKey);
        return ApiResponse<DepartmentDto>.Ok(_mapper.Map<DepartmentDto>(dept));
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id)
    {
        var dept = await _context.Departments.FindAsync(id);
        if (dept is null) return ApiResponse<bool>.NotFound("Department not found.");

        dept.IsDeleted = true;
        await _context.SaveChangesAsync();
        _cache.Remove(CacheKey);
        return ApiResponse<bool>.Ok(true, "Department deleted.");
    }
}

// ── Bill Service ──────────────────────────────────────────────────────────────
public class BillService : IBillService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public BillService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper  = mapper;
    }

    public async Task<ApiResponse<BillDto>> GetByIdAsync(int id)
    {
        var bill = await IncludeQuery().FirstOrDefaultAsync(b => b.Id == id);
        return bill is null
            ? ApiResponse<BillDto>.NotFound("Bill not found.")
            : ApiResponse<BillDto>.Ok(MapBill(bill));
    }

    public async Task<ApiResponse<BillDto>> GetByAppointmentIdAsync(int appointmentId)
    {
        var bill = await IncludeQuery().FirstOrDefaultAsync(b => b.AppointmentId == appointmentId);
        return bill is null
            ? ApiResponse<BillDto>.NotFound("No bill found for this appointment.")
            : ApiResponse<BillDto>.Ok(MapBill(bill));
    }

    public async Task<ApiResponse<List<BillDto>>> GetAllAsync(string? paymentStatus = null)
    {
        var query = IncludeQuery();
        if (!string.IsNullOrWhiteSpace(paymentStatus))
            query = query.Where(b => b.PaymentStatus == paymentStatus);

        var bills = await query.OrderByDescending(b => b.CreatedAt).ToListAsync();
        return ApiResponse<List<BillDto>>.Ok(bills.Select(MapBill).ToList());
    }

    public async Task<ApiResponse<BillDto>> CreateAsync(CreateBillDto dto)
    {
        // Verify appointment exists
        var appt = await _context.Appointments.FindAsync(dto.AppointmentId);
        if (appt is null)
            return ApiResponse<BillDto>.NotFound("Appointment not found.");

        if (await _context.Bills.AnyAsync(b => b.AppointmentId == dto.AppointmentId))
            return ApiResponse<BillDto>.Fail("A bill already exists for this appointment.");

        var bill = _mapper.Map<Bill>(dto);
        _context.Bills.Add(bill);
        await _context.SaveChangesAsync();

        // Auto-update appointment IsPaid flag if fee is set
        return await GetByIdAsync(bill.Id);
    }

    public async Task<ApiResponse<BillDto>> UpdatePaymentStatusAsync(int id, UpdateBillPaymentDto dto)
    {
        var bill = await _context.Bills
            .Include(b => b.Appointment)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (bill is null) return ApiResponse<BillDto>.NotFound("Bill not found.");

        bill.PaymentStatus = dto.PaymentStatus;

        // Keep Appointment.IsPaid in sync
        bill.Appointment.IsPaid = dto.PaymentStatus == "Paid";
        await _context.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private IQueryable<Bill> IncludeQuery() =>
        _context.Bills
            .Include(b => b.Appointment)
                .ThenInclude(a => a.PatientProfile).ThenInclude(p => p.User)
            .Include(b => b.Appointment)
                .ThenInclude(a => a.DoctorProfile).ThenInclude(d => d.User)
            .AsQueryable();

    private BillDto MapBill(Bill b) => new()
    {
        Id               = b.Id,
        AppointmentId    = b.AppointmentId,
        PatientName      = b.Appointment.PatientProfile.User.FullName,
        DoctorName       = b.Appointment.DoctorProfile.User.FullName,
        AppointmentDate  = b.Appointment.AppointmentDate,
        ConsultationFee  = b.ConsultationFee,
        MedicineCharges  = b.MedicineCharges,
        TotalAmount      = b.ConsultationFee + b.MedicineCharges,
        PaymentStatus    = b.PaymentStatus,
        Notes            = b.Notes,
        CreatedAt        = b.CreatedAt
    };
}
