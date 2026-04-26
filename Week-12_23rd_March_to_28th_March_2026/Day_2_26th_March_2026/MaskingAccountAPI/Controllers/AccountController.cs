using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MaskingAccountAPI.Data;
using MaskingAccountAPI.DTOs;

namespace MaskingAccountAPI.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public AccountController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("masked")]
        public async Task<IActionResult> GetAllMaskedAccounts()
        {
            var accounts = await _context.Accounts.ToListAsync();

            var result = _mapper.Map<List<MaskedAccountDto>>(accounts);

            return Ok(result);
        }

        [HttpGet("masked/{id}")]
        public async Task<IActionResult> GetMaskedAccountById(int id)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == id);

            if (account == null)
                return NotFound(new { message = "Account not found." });

            var result = _mapper.Map<MaskedAccountDto>(account);

            return Ok(result);
        }
    }
}
