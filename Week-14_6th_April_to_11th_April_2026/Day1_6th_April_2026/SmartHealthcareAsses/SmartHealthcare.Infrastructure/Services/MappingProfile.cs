using AutoMapper;
using SmartHealthcare.Core.DTOs;
using SmartHealthcare.Core.Entities;

namespace SmartHealthcare.Infrastructure.Services;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // ── Department ──────────────────────────────────────────────────────
        CreateMap<Department, DepartmentDto>()
            .ForMember(d => d.DoctorCount, o => o.MapFrom(s => s.Doctors.Count));
        CreateMap<CreateDepartmentDto, Department>();
        CreateMap<UpdateDepartmentDto, Department>();

        // ── Patient ─────────────────────────────────────────────────────────
        CreateMap<PatientProfile, PatientDto>()
            .ForMember(d => d.FullName,      o => o.MapFrom(s => s.User.FullName))
            .ForMember(d => d.Email,         o => o.MapFrom(s => s.User.Email))
            .ForMember(d => d.PhoneNumber,   o => o.MapFrom(s => s.User.PhoneNumber));
        CreateMap<CreatePatientDto, PatientProfile>();
        CreateMap<UpdatePatientDto, PatientProfile>();

        // ── Doctor ──────────────────────────────────────────────────────────
        CreateMap<DoctorProfile, DoctorDto>()
            .ForMember(d => d.FullName,        o => o.MapFrom(s => s.User.FullName))
            .ForMember(d => d.Email,           o => o.MapFrom(s => s.User.Email))
            .ForMember(d => d.PhoneNumber,     o => o.MapFrom(s => s.User.PhoneNumber))
            .ForMember(d => d.DepartmentName,  o => o.MapFrom(s => s.Department != null ? s.Department.DepartmentName : null))
            .ForMember(d => d.Specializations, o => o.MapFrom(s =>
                s.DoctorSpecializations.Select(ds => new SpecializationDto
                {
                    Id          = ds.SpecializationId,
                    Name        = ds.Specialization.Name,
                    Description = ds.Specialization.Description,
                    IsPrimary   = ds.IsPrimary
                }).ToList()));
        CreateMap<CreateDoctorDto, DoctorProfile>();
        CreateMap<UpdateDoctorDto, DoctorProfile>();

        // ── Specialization ──────────────────────────────────────────────────
        CreateMap<Specialization, SpecializationDto>();
        CreateMap<CreateSpecializationDto, Specialization>();

        // ── Appointment ─────────────────────────────────────────────────────
        CreateMap<Appointment, AppointmentDto>()
            .ForMember(d => d.PatientName,          o => o.MapFrom(s => s.PatientProfile.User.FullName))
            .ForMember(d => d.DoctorName,           o => o.MapFrom(s => s.DoctorProfile.User.FullName))
            .ForMember(d => d.DoctorSpecializations, o => o.MapFrom(s =>
                string.Join(", ", s.DoctorProfile.DoctorSpecializations
                    .Select(ds => ds.Specialization.Name))))
            .ForMember(d => d.HasPrescription,      o => o.MapFrom(s => s.Prescription != null));
        CreateMap<CreateAppointmentDto, Appointment>();
        CreateMap<UpdateAppointmentDto, Appointment>();

        // ── Prescription ────────────────────────────────────────────────────
        CreateMap<Prescription, PrescriptionDto>()
            .ForMember(d => d.PatientName, o => o.MapFrom(s => s.Appointment.PatientProfile.User.FullName))
            .ForMember(d => d.DoctorName,  o => o.MapFrom(s => s.Appointment.DoctorProfile.User.FullName))
            .ForMember(d => d.Medicines,   o => o.MapFrom(s => s.PrescriptionMedicines));
        CreateMap<CreatePrescriptionDto, Prescription>();

        CreateMap<PrescriptionMedicine, PrescriptionMedicineDto>()
            .ForMember(d => d.MedicineName, o => o.MapFrom(s => s.Medicine.Name));
        CreateMap<CreatePrescriptionMedicineDto, PrescriptionMedicine>();

        // ── Medicine ────────────────────────────────────────────────────────
        CreateMap<Medicine, MedicineDto>();
        CreateMap<CreateMedicineDto, Medicine>();

        // ── Bill ────────────────────────────────────────────────────────────
        CreateMap<Bill, BillDto>()
            .ForMember(d => d.PatientName,     o => o.MapFrom(s => s.Appointment.PatientProfile.User.FullName))
            .ForMember(d => d.DoctorName,      o => o.MapFrom(s => s.Appointment.DoctorProfile.User.FullName))
            .ForMember(d => d.AppointmentDate, o => o.MapFrom(s => s.Appointment.AppointmentDate))
            .ForMember(d => d.TotalAmount,     o => o.MapFrom(s => s.ConsultationFee + s.MedicineCharges));
        CreateMap<CreateBillDto, Bill>();
    }
}
