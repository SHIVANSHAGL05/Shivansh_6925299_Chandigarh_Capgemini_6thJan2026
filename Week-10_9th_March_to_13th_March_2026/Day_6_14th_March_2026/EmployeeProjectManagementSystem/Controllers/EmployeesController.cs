using EmployeeProjectManagementSystem.Data;
using EmployeeProjectManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeProjectManagementSystem.Controllers;

public class EmployeesController : Controller
{
    private readonly AppDbContext _context;

    public EmployeesController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var employees = await _context.Employees
            .Include(e => e.Department)
            .Include(e => e.EmployeeProjects)
                .ThenInclude(ep => ep.Project)
            .ToListAsync();

        return View(employees);
    }

    public async Task<IActionResult> Details(int id)
    {
        var employee = await _context.Employees
            .Include(e => e.Department)
            .Include(e => e.EmployeeProjects)
                .ThenInclude(ep => ep.Project)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (employee == null) return NotFound();

        return View(employee);
    }

    public IActionResult Create()
    {
        ViewBag.Departments = _context.Departments.ToList();
        ViewBag.Projects = _context.Projects.ToList();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Employee employee, int[] selectedProjects)
    {
        ModelState.Remove("Department");
        ModelState.Remove("EmployeeProjects");

        if (ModelState.IsValid)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            foreach (var projectId in selectedProjects)
            {
                _context.EmployeeProjects.Add(new EmployeeProject
                {
                    EmployeeId = employee.Id,
                    ProjectId = projectId,
                    AssignedDate = DateTime.Now
                });
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Departments = _context.Departments.ToList();
        ViewBag.Projects = _context.Projects.ToList();
        return View(employee);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var employee = await _context.Employees
            .Include(e => e.EmployeeProjects)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (employee == null) return NotFound();

        ViewBag.Departments = _context.Departments.ToList();
        ViewBag.Projects = _context.Projects.ToList();
        ViewBag.AssignedProjectIds = employee.EmployeeProjects.Select(ep => ep.ProjectId).ToList();
        return View(employee);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Employee employee, int[] selectedProjects)
    {
        if (id != employee.Id) return NotFound();

        ModelState.Remove("Department");
        ModelState.Remove("EmployeeProjects");

        if (ModelState.IsValid)
        {
            _context.Update(employee);

            var existing = _context.EmployeeProjects.Where(ep => ep.EmployeeId == id);
            _context.EmployeeProjects.RemoveRange(existing);

            foreach (var projectId in selectedProjects)
            {
                _context.EmployeeProjects.Add(new EmployeeProject
                {
                    EmployeeId = id,
                    ProjectId = projectId,
                    AssignedDate = DateTime.Now
                });
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Departments = _context.Departments.ToList();
        ViewBag.Projects = _context.Projects.ToList();
        return View(employee);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var employee = await _context.Employees
            .Include(e => e.Department)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (employee == null) return NotFound();

        return View(employee);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee != null) _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> EmployeeProjects(int id)
    {
        var projects = await _context.EmployeeProjects
            .Where(ep => ep.EmployeeId == id)
            .Include(ep => ep.Project)
            .ToListAsync();

        ViewBag.EmployeeName = (await _context.Employees.FindAsync(id))?.Name;
        return View(projects);
    }
}
