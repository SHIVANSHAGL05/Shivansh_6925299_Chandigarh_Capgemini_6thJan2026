using SmartHealthcare.Models.Entities;

namespace SmartHealthcare.API.Repositories.Interfaces;

public interface IPrescriptionRepository : IGenericRepository<Prescription>
{
    Task<Prescription?> GetByAppointmentIdAsync(int appointmentId);
    Task<Prescription?> GetWithMedicinesAsync(int id);
}
