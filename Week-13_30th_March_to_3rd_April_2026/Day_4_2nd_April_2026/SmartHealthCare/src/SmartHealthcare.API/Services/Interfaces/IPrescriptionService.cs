using SmartHealthcare.Models.DTOs;

namespace SmartHealthcare.API.Services.Interfaces;

public interface IPrescriptionService
{
    Task<PrescriptionDTO?> GetByAppointmentIdAsync(int appointmentId);
    Task<PrescriptionDTO> CreateAsync(CreatePrescriptionDTO dto);
}