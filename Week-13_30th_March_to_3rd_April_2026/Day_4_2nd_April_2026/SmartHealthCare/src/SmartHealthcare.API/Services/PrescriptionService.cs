using AutoMapper;
using SmartHealthcare.API.Repositories.Interfaces;
using SmartHealthcare.API.Services.Interfaces;
using SmartHealthcare.Models.DTOs;
using SmartHealthcare.Models.Entities;

namespace SmartHealthcare.API.Services;

public class PrescriptionService : IPrescriptionService
{
    private readonly IPrescriptionRepository _repo;
    private readonly IMapper _mapper;
    private readonly ILogger<PrescriptionService> _logger;

    public PrescriptionService(IPrescriptionRepository repo, IMapper mapper, ILogger<PrescriptionService> logger)
    {
        _repo = repo;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PrescriptionDTO?> GetByAppointmentIdAsync(int appointmentId)
    {
        var prescription = await _repo.GetByAppointmentIdAsync(appointmentId);
        return prescription == null ? null : _mapper.Map<PrescriptionDTO>(prescription);
    }

    public async Task<PrescriptionDTO> CreateAsync(CreatePrescriptionDTO dto)
    {
        var prescription = _mapper.Map<Prescription>(dto);
        prescription.CreatedAt = DateTime.UtcNow;

        foreach (var medDto in dto.Medicines)
        {
            prescription.Medicines.Add(_mapper.Map<Medicine>(medDto));
        }

        await _repo.AddAsync(prescription);
        await _repo.SaveAsync();
        _logger.LogInformation("Prescription created for AppointmentId: {AppointmentId}", dto.AppointmentId);

        var created = await _repo.GetWithMedicinesAsync(prescription.Id);
        return _mapper.Map<PrescriptionDTO>(created);
    }
}