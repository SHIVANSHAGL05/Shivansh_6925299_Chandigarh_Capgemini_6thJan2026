using EmployeeProjectManagementSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeProjectManagementSystem.Controllers;

public class DepartmentSummary
{
    public string DepartmentName { get; set; } = string.Empty;
    public int EmployeeCount { get; set; }
}

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var employeesPerDepartment = await _context.Departments
            .Include(d => d.Employees)
            .Select(d => new DepartmentSummary
            {
                DepartmentName = d.Name,
                EmployeeCount = d.Employees.Count
            })
            .ToListAsync();

        ViewBag.EmployeesPerDepartment = employeesPerDepartment;
        return View();
    }
}
