using AutoMapper;
using SmartHealthcare.API.Repositories.Interfaces;
using SmartHealthcare.API.Services.Interfaces;
using SmartHealthcare.Models.DTOs;
using SmartHealthcare.Models.Entities;

namespace SmartHealthcare.API.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _repo;
    private readonly IMapper _mapper;

    public DepartmentService(IDepartmentRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<PagedResult<DepartmentDTO>> GetAllAsync(int page, int pageSize)
    {
        var items = await _repo.GetAllWithDoctorsAsync(page, pageSize);
        var total = await _repo.CountAsync();
        return new PagedResult<DepartmentDTO>
        {
            Items = _mapper.Map<List<DepartmentDTO>>(items),
            TotalCount = total,
            PageNumber = page,
            PageSize = pageSize
        };
    }

    public async Task<DepartmentDTO?> GetByIdAsync(int id)
    {
        var dep = await _repo.GetWithDoctorsAsync(id);
        return dep == null ? null : _mapper.Map<DepartmentDTO>(dep);
    }

    public async Task<DepartmentDTO> CreateAsync(CreateDepartmentDTO dto)
    {
        var dep = _mapper.Map<Department>(dto);
        await _repo.AddAsync(dep);
        await _repo.SaveAsync();
        var created = await _repo.GetWithDoctorsAsync(dep.Id);
        return _mapper.Map<DepartmentDTO>(created);
    }

    public async Task<bool> UpdateAsync(int id, UpdateDepartmentDTO dto)
    {
        var dep = await _repo.GetByIdAsync(id);
        if (dep == null)
        {
            return false;
        }

        _mapper.Map(dto, dep);
        _repo.Update(dep);
        await _repo.SaveAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var dep = await _repo.GetByIdAsync(id);
        if (dep == null)
        {
            return false;
        }

        _repo.Delete(dep);
        await _repo.SaveAsync();
        return true;
    }
}
