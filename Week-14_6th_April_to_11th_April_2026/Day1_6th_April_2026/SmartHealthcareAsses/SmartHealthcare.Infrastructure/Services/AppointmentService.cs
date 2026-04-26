using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartHealthcare.Core.Common;
using SmartHealthcare.Core.DTOs;
using SmartHealthcare.Core.Entities;
using SmartHealthcare.Core.Enums;
using SmartHealthcare.Core.Interfaces;
using SmartHealthcare.Infrastructure.Data;

namespace SmartHealthcare.Infrastructure.Services;

public class AppointmentService : IAppointmentService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public AppointmentService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper  = mapper;
    }

    public async Task<ApiResponse<AppointmentDto>> GetByIdAsync(int id)
    {
        var appt = await IncludeQuery().FirstOrDefaultAsync(a => a.Id == id);
        return appt is null
            ? ApiResponse<AppointmentDto>.NotFound("Appointment not found.")
            : ApiResponse<AppointmentDto>.Ok(_mapper.Map<AppointmentDto>(appt));
    }

    public async Task<ApiResponse<PagedResult<AppointmentDto>>> GetAllAsync(AppointmentFilterDto filter)
    {
        var query = IncludeQuery();

        if (filter.Date.HasValue)
            query = query.Where(a => a.AppointmentDate.Date == filter.Date.Value.Date);
        if (filter.Status.HasValue)
            query = query.Where(a => a.Status == filter.Status.Value);
        if (filter.DoctorProfileId.HasValue)
            query = query.Where(a => a.DoctorProfileId == filter.DoctorProfileId.Value);
        if (filter.PatientProfileId.HasValue)
            query = query.Where(a => a.PatientProfileId == filter.PatientProfileId.Value);

        var total = await query.CountAsync();
        var items = await query.OrderByDescending(a => a.AppointmentDate)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return ApiResponse<PagedResult<AppointmentDto>>.Ok(new PagedResult<AppointmentDto>
        {
            Items      = _mapper.Map<List<AppointmentDto>>(items),
            TotalCount = total,
            Page       = filter.Page,
            PageSize   = filter.PageSize
        });
    }

    public async Task<ApiResponse<AppointmentDto>> CreateAsync(int patientUserId, CreateAppointmentDto dto)
    {
        var patient = await _context.PatientProfiles
            .FirstOrDefaultAsync(p => p.UserId == patientUserId);
        if (patient is null)
            return ApiResponse<AppointmentDto>.NotFound("Patient profile not found.");

        var doctor = await _context.DoctorProfiles.FindAsync(dto.DoctorProfileId);
        if (doctor is null || !doctor.IsAvailable)
            return ApiResponse<AppointmentDto>.NotFound("Doctor not found or unavailable.");

        // Conflict check
        var conflict = await _context.Appointments.AnyAsync(a =>
            a.DoctorProfileId == dto.DoctorProfileId &&
            a.AppointmentDate.Date == dto.AppointmentDate.Date &&
            a.StartTime == dto.StartTime &&
            a.Status != AppointmentStatus.Cancelled);

        if (conflict)
            return ApiResponse<AppointmentDto>.Fail("This time slot is already booked.");

        var appt = _mapper.Map<Appointment>(dto);
        appt.PatientProfileId = patient.Id;
        appt.EndTime          = dto.StartTime.Add(TimeSpan.FromMinutes(30));
        appt.Fee              = doctor.ConsultationFee;

        _context.Appointments.Add(appt);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(appt.Id);
    }

    public async Task<ApiResponse<AppointmentDto>> UpdateAsync(int id, UpdateAppointmentDto dto, int requestingUserId)
    {
        var appt = await _context.Appointments
            .Include(a => a.PatientProfile)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (appt is null)
            return ApiResponse<AppointmentDto>.NotFound("Appointment not found.");

        if (appt.PatientProfile.UserId != requestingUserId)
            return ApiResponse<AppointmentDto>.Unauthorized("You cannot modify this appointment.");

        if (appt.Status != AppointmentStatus.Pending)
            return ApiResponse<AppointmentDto>.Fail("Only pending appointments can be updated.");

        _mapper.Map(dto, appt);
        appt.EndTime = dto.StartTime.Add(TimeSpan.FromMinutes(30));
        await _context.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task<ApiResponse<AppointmentDto>> UpdateStatusAsync(int id, UpdateAppointmentStatusDto dto, int requestingUserId)
    {
        var appt = await _context.Appointments
            .Include(a => a.DoctorProfile)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (appt is null)
            return ApiResponse<AppointmentDto>.NotFound("Appointment not found.");

        if (appt.DoctorProfile.UserId != requestingUserId)
            return ApiResponse<AppointmentDto>.Unauthorized("Only the assigned doctor can update status.");

        appt.Status      = dto.Status;
        appt.DoctorNotes = dto.DoctorNotes;
        await _context.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task<ApiResponse<bool>> CancelAsync(int id, int requestingUserId)
    {
        var appt = await _context.Appointments
            .Include(a => a.PatientProfile)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (appt is null)
            return ApiResponse<bool>.NotFound("Appointment not found.");

        if (appt.PatientProfile.UserId != requestingUserId)
            return ApiResponse<bool>.Unauthorized("You cannot cancel this appointment.");

        if (appt.Status == AppointmentStatus.Completed)
            return ApiResponse<bool>.Fail("Completed appointments cannot be cancelled.");

        appt.Status = AppointmentStatus.Cancelled;
        await _context.SaveChangesAsync();
        return ApiResponse<bool>.Ok(true, "Appointment cancelled.");
    }

    public async Task<ApiResponse<List<AppointmentDto>>> GetByPatientAsync(int patientProfileId)
    {
        var appts = await IncludeQuery()
            .Where(a => a.PatientProfileId == patientProfileId)
            .OrderByDescending(a => a.AppointmentDate)
            .ToListAsync();

        return ApiResponse<List<AppointmentDto>>.Ok(_mapper.Map<List<AppointmentDto>>(appts));
    }

    public async Task<ApiResponse<List<AppointmentDto>>> GetByDoctorAsync(int doctorProfileId, DateTime? date = null)
    {
        var query = IncludeQuery().Where(a => a.DoctorProfileId == doctorProfileId);
        if (date.HasValue)
            query = query.Where(a => a.AppointmentDate.Date == date.Value.Date);

        var appts = await query.OrderBy(a => a.AppointmentDate).ThenBy(a => a.StartTime).ToListAsync();
        return ApiResponse<List<AppointmentDto>>.Ok(_mapper.Map<List<AppointmentDto>>(appts));
    }

    private IQueryable<Appointment> IncludeQuery() =>
        _context.Appointments
            .Include(a => a.PatientProfile).ThenInclude(p => p.User)
            .Include(a => a.DoctorProfile).ThenInclude(d => d.User)
            .Include(a => a.DoctorProfile).ThenInclude(d => d.DoctorSpecializations).ThenInclude(ds => ds.Specialization)
            .Include(a => a.Prescription)
            .AsQueryable();
}
