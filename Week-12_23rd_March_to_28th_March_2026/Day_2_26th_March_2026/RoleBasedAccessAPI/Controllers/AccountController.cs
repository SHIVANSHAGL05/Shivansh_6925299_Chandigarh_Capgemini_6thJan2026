using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoleBasedAccessAPI.Data;
using RoleBasedAccessAPI.DTOs;
using System.Security.Claims;

namespace RoleBasedAccessAPI.Controllers
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

        [HttpGet("details")]
        [Authorize]
        public async Task<IActionResult> GetAccountDetails()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role);

            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.UserId == userId);

            if (account == null)
                return NotFound(new { message = "Account not found." });

            if (role == "Admin")
            {
                var adminDto = _mapper.Map<AdminAccountDto>(account);
                return Ok(adminDto);
            }

            var userDto = _mapper.Map<UserAccountDto>(account);
            return Ok(userDto);
        }

        [HttpGet("admin/all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllAccounts()
        {
            var accounts = await _context.Accounts.ToListAsync();
            var result = _mapper.Map<List<AdminAccountDto>>(accounts);
            return Ok(result);
        }
    }
}
