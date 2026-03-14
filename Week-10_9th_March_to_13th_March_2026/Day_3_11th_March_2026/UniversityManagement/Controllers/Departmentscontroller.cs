// Controllers/DepartmentsController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityManagement.Data;
using UniversityManagement.Models;

namespace UniversityManagement.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly UniversityContext _context;

        public DepartmentsController(UniversityContext context)
            => _context = context;

        // GET: /Departments
        public async Task<IActionResult> Index()
        {
            var departments = await _context.Departments
                .Include(d => d.Instructors)
                    .ThenInclude(i => i.Courses)
                .OrderBy(d => d.Name)
                .ToListAsync();

            return View(departments);
        }

        // GET: /Departments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var department = await _context.Departments
                .Include(d => d.Instructors)
                    .ThenInclude(i => i.Courses)
                .FirstOrDefaultAsync(d => d.DepartmentId == id);

            if (department == null) return NotFound();
            return View(department);
        }

        // GET: /Departments/Create
        public IActionResult Create() => View();

        // POST: /Departments/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Name,Budget")] Department department)
        {
            if (ModelState.IsValid)
            {
                _context.Add(department);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(department);
        }

        // GET: /Departments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var department = await _context.Departments.FindAsync(id);
            if (department == null) return NotFound();
            return View(department);
        }

        // POST: /Departments/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("DepartmentId,Name,Budget")] Department department)
        {
            if (id != department.DepartmentId) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(department);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(department);
        }

        // GET: /Departments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var department = await _context.Departments
                .Include(d => d.Instructors)
                .FirstOrDefaultAsync(d => d.DepartmentId == id);

            if (department == null) return NotFound();
            return View(department);
        }

        // POST: /Departments/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var department = await _context.Departments
                .Include(d => d.Instructors)
                .FirstOrDefaultAsync(d => d.DepartmentId == id);

            if (department == null) return NotFound();

            // Block delete if instructors are still assigned
            if (department.Instructors.Any())
            {
                ModelState.AddModelError("",
                    "Cannot delete a department that still has instructors assigned. Remove or reassign them first.");
                return View(department);
            }

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}