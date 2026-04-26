using Microsoft.EntityFrameworkCore;
using SmartHealthcare.API.Data;
using SmartHealthcare.API.Repositories.Interfaces;
using SmartHealthcare.Models.Entities;

namespace SmartHealthcare.API.Repositories;

public class PrescriptionRepository : GenericRepository<Prescription>, IPrescriptionRepository
{
    public PrescriptionRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Prescription?> GetByAppointmentIdAsync(int appointmentId)
        => await _dbSet.Include(p => p.Medicines)
                       .FirstOrDefaultAsync(p => p.AppointmentId == appointmentId);

    public async Task<Prescription?> GetWithMedicinesAsync(int id)
        => await _dbSet.Include(p => p.Medicines)
                       .FirstOrDefaultAsync(p => p.Id == id);
}
