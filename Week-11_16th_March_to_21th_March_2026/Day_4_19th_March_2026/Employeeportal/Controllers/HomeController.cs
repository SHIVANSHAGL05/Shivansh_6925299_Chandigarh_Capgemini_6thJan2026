using EmployeePortal.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeePortal.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Dashboard()
        {
            ViewData["TotalEmployees"] = await _context.Employees.CountAsync();
            ViewData["TotalDepartments"] = await _context.Employees
                .Select(e => e.Department)
                .Distinct()
                .CountAsync();
            return View();
        }

        [Authorize]
        public IActionResult Profile()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
