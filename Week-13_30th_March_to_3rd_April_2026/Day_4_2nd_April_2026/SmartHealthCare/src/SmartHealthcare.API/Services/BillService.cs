using AutoMapper;
using SmartHealthcare.API.Repositories.Interfaces;
using SmartHealthcare.API.Services.Interfaces;
using SmartHealthcare.Models.DTOs;
using SmartHealthcare.Models.Entities;

namespace SmartHealthcare.API.Services;

public class BillService : IBillService
{
    private readonly IBillRepository _repo;
    private readonly IMapper _mapper;

    public BillService(IBillRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<PagedResult<BillDTO>> GetAllAsync(int page, int pageSize)
    {
        var items = await _repo.GetAllWithDetailsAsync(page, pageSize);
        var total = await _repo.CountAsync();
        return new PagedResult<BillDTO>
        {
            Items = _mapper.Map<List<BillDTO>>(items),
            TotalCount = total,
            PageNumber = page,
            PageSize = pageSize
        };
    }

    public async Task<BillDTO?> GetByIdAsync(int id)
    {
        var bill = await _repo.GetWithDetailsAsync(id);
        return bill == null ? null : _mapper.Map<BillDTO>(bill);
    }

    public async Task<BillDTO?> GetByAppointmentIdAsync(int appointmentId)
    {
        var bill = await _repo.GetByAppointmentIdAsync(appointmentId);
        return bill == null ? null : _mapper.Map<BillDTO>(bill);
    }

    public async Task<BillDTO> CreateAsync(CreateBillDTO dto)
    {
        var bill = _mapper.Map<Bill>(dto);
        await _repo.AddAsync(bill);
        await _repo.SaveAsync();
        var created = await _repo.GetWithDetailsAsync(bill.Id);
        return _mapper.Map<BillDTO>(created);
    }

    public async Task<bool> UpdateAsync(int id, UpdateBillDTO dto)
    {
        var bill = await _repo.GetByIdAsync(id);
        if (bill == null)
        {
            return false;
        }

        _mapper.Map(dto, bill);
        _repo.Update(bill);
        await _repo.SaveAsync();
        return true;
    }

    public async Task<bool> MarkAsPaidAsync(int id, PayBillDTO dto)
    {
        var bill = await _repo.GetByIdAsync(id);
        if (bill == null)
        {
            return false;
        }

        if (bill.PaymentStatus == "Paid")
        {
            return true;
        }

        bill.PaymentStatus = "Paid";
        bill.PaidAt = DateTime.UtcNow;
        bill.TransactionReference = dto.TransactionReference;

        _repo.Update(bill);
        await _repo.SaveAsync();
        return true;
    }
}
