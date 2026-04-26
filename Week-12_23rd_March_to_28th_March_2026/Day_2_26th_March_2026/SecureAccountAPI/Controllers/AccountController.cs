using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureAccountAPI.Data;
using SecureAccountAPI.DTOs;
using System.Security.Claims;

namespace SecureAccountAPI.Controllers
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

            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.UserId == userId);

            if (account == null)
                return NotFound(new { message = "Account not found." });

            var dto = _mapper.Map<AccountDetailsDto>(account);

            return Ok(dto);
        }
    }
}
