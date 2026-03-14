using EmployeeProjectManagementSystem.Data;
using EmployeeProjectManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeProjectManagementSystem.Controllers;

public class ProjectsController : Controller
{
    private readonly AppDbContext _context;

    public ProjectsController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var projects = await _context.Projects
            .Include(p => p.EmployeeProjects)
            .ToListAsync();

        return View(projects);
    }

    public async Task<IActionResult> Details(int id)
    {
        var project = await _context.Projects
            .Include(p => p.EmployeeProjects)
                .ThenInclude(ep => ep.Employee)
                    .ThenInclude(e => e!.Department)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (project == null) return NotFound();

        return View(project);
    }

    public IActionResult Create()
    {
        ViewBag.Employees = _context.Employees.Include(e => e.Department).ToList();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Project project, int[] selectedEmployees)
    {
        if (ModelState.IsValid)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            foreach (var employeeId in selectedEmployees)
            {
                _context.EmployeeProjects.Add(new EmployeeProject
                {
                    ProjectId = project.Id,
                    EmployeeId = employeeId,
                    AssignedDate = DateTime.Now
                });
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Employees = _context.Employees.Include(e => e.Department).ToList();
        return View(project);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var project = await _context.Projects
            .Include(p => p.EmployeeProjects)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (project == null) return NotFound();

        ViewBag.Employees = _context.Employees.Include(e => e.Department).ToList();
        ViewBag.AssignedEmployeeIds = project.EmployeeProjects.Select(ep => ep.EmployeeId).ToList();
        return View(project);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Project project, int[] selectedEmployees)
    {
        if (id != project.Id) return NotFound();

        if (ModelState.IsValid)
        {
            _context.Update(project);

            var existing = _context.EmployeeProjects.Where(ep => ep.ProjectId == id);
            _context.EmployeeProjects.RemoveRange(existing);

            foreach (var employeeId in selectedEmployees)
            {
                _context.EmployeeProjects.Add(new EmployeeProject
                {
                    ProjectId = id,
                    EmployeeId = employeeId,
                    AssignedDate = DateTime.Now
                });
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Employees = _context.Employees.Include(e => e.Department).ToList();
        return View(project);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project == null) return NotFound();
        return View(project);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project != null) _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> ProjectEmployees(int id)
    {
        var employees = await _context.EmployeeProjects
            .Where(ep => ep.ProjectId == id)
            .Include(ep => ep.Employee)
                .ThenInclude(e => e!.Department)
            .ToListAsync();

        ViewBag.ProjectTitle = (await _context.Projects.FindAsync(id))?.Title;
        return View(employees);
    }
    public async Task<IActionResult> Dashboard()
    {
        var projects = await _context.Projects
            .Include(p => p.EmployeeProjects)
                .ThenInclude(ep => ep.Employee)
                    .ThenInclude(e => e!.Department)
            .ToListAsync();

        return View(projects);
    }
}
