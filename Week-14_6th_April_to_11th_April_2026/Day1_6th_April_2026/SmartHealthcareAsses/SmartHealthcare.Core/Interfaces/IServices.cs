using SmartHealthcare.Core.Common;
using SmartHealthcare.Core.DTOs;

namespace SmartHealthcare.Core.Interfaces;

public interface IAuthService
{
    Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterDto dto, string ipAddress);
    Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto dto, string ipAddress);
    Task<ApiResponse<AuthResponseDto>> RefreshTokenAsync(string refreshToken, string ipAddress);
    Task<ApiResponse<bool>> RevokeTokenAsync(string refreshToken, string ipAddress);
}

public interface IPatientService
{
    Task<ApiResponse<PatientDto>> GetByIdAsync(int id);
    Task<ApiResponse<PatientDto>> GetByUserIdAsync(int userId);
    Task<ApiResponse<PagedResult<PatientDto>>> GetAllAsync(int page, int pageSize, string? search = null);
    Task<ApiResponse<PatientDto>> CreateAsync(int userId, CreatePatientDto dto);
    Task<ApiResponse<PatientDto>> UpdateAsync(int id, UpdatePatientDto dto);
    Task<ApiResponse<bool>> DeleteAsync(int id);
}

public interface IDoctorService
{
    Task<ApiResponse<DoctorDto>> GetByIdAsync(int id);
    Task<ApiResponse<DoctorDto>> GetByUserIdAsync(int userId);
    Task<ApiResponse<PagedResult<DoctorDto>>> GetAllAsync(int page, int pageSize, string? search = null, int? specializationId = null);
    Task<ApiResponse<DoctorDto>> CreateAsync(int userId, CreateDoctorDto dto);
    Task<ApiResponse<DoctorDto>> UpdateAsync(int id, UpdateDoctorDto dto);
    Task<ApiResponse<bool>> DeleteAsync(int id);
    Task<ApiResponse<List<DoctorDto>>> GetBySpecializationAsync(int specializationId);
}

public interface IAppointmentService
{
    Task<ApiResponse<AppointmentDto>> GetByIdAsync(int id);
    Task<ApiResponse<PagedResult<AppointmentDto>>> GetAllAsync(AppointmentFilterDto filter);
    Task<ApiResponse<AppointmentDto>> CreateAsync(int patientUserId, CreateAppointmentDto dto);
    Task<ApiResponse<AppointmentDto>> UpdateAsync(int id, UpdateAppointmentDto dto, int requestingUserId);
    Task<ApiResponse<AppointmentDto>> UpdateStatusAsync(int id, UpdateAppointmentStatusDto dto, int requestingUserId);
    Task<ApiResponse<bool>> CancelAsync(int id, int requestingUserId);
    Task<ApiResponse<List<AppointmentDto>>> GetByPatientAsync(int patientProfileId);
    Task<ApiResponse<List<AppointmentDto>>> GetByDoctorAsync(int doctorProfileId, DateTime? date = null);
}

public interface IPrescriptionService
{
    Task<ApiResponse<PrescriptionDto>> GetByIdAsync(int id);
    Task<ApiResponse<PrescriptionDto>> GetByAppointmentIdAsync(int appointmentId);
    Task<ApiResponse<List<PrescriptionDto>>> GetByPatientAsync(int patientProfileId);
    Task<ApiResponse<PrescriptionDto>> CreateAsync(CreatePrescriptionDto dto, int doctorUserId);
}

public interface IMedicineService
{
    Task<ApiResponse<List<MedicineDto>>> GetAllAsync(string? search = null);
    Task<ApiResponse<MedicineDto>> GetByIdAsync(int id);
    Task<ApiResponse<MedicineDto>> CreateAsync(CreateMedicineDto dto);
    Task<ApiResponse<MedicineDto>> UpdateAsync(int id, CreateMedicineDto dto);
    Task<ApiResponse<bool>> DeleteAsync(int id);
}

public interface ISpecializationService
{
    Task<ApiResponse<List<SpecializationDto>>> GetAllAsync();
    Task<ApiResponse<SpecializationDto>> GetByIdAsync(int id);
    Task<ApiResponse<SpecializationDto>> CreateAsync(CreateSpecializationDto dto);
    Task<ApiResponse<bool>> DeleteAsync(int id);
}

public interface IDepartmentService
{
    Task<ApiResponse<List<DepartmentDto>>> GetAllAsync();
    Task<ApiResponse<DepartmentDto>> GetByIdAsync(int id);
    Task<ApiResponse<List<DoctorDto>>> GetDoctorsByDepartmentAsync(int departmentId);
    Task<ApiResponse<DepartmentDto>> CreateAsync(CreateDepartmentDto dto);
    Task<ApiResponse<DepartmentDto>> UpdateAsync(int id, UpdateDepartmentDto dto);
    Task<ApiResponse<bool>> DeleteAsync(int id);
}

public interface IBillService
{
    Task<ApiResponse<BillDto>> GetByIdAsync(int id);
    Task<ApiResponse<BillDto>> GetByAppointmentIdAsync(int appointmentId);
    Task<ApiResponse<List<BillDto>>> GetAllAsync(string? paymentStatus = null);
    Task<ApiResponse<BillDto>> CreateAsync(CreateBillDto dto);
    Task<ApiResponse<BillDto>> UpdatePaymentStatusAsync(int id, UpdateBillPaymentDto dto);
}
