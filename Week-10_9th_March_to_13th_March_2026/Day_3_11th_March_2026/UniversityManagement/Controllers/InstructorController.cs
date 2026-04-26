// Controllers/InstructorsController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UniversityManagement.Data;
using UniversityManagement.Models;

namespace UniversityManagement.Controllers
{
    public class InstructorsController : Controller
    {
        private readonly UniversityContext _context;

        public InstructorsController(UniversityContext context)
            => _context = context;

        // GET: /Instructors
        public async Task<IActionResult> Index()
        {
            var instructors = await _context.Instructors
                .Include(i => i.Department)
                .Include(i => i.Courses)
                .OrderBy(i => i.Name)
                .ToListAsync();

            return View(instructors);
        }

        // GET: /Instructors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var instructor = await _context.Instructors
                .Include(i => i.Department)
                .Include(i => i.Courses)
                    .ThenInclude(c => c.Enrollments)
                .FirstOrDefaultAsync(i => i.InstructorId == id);

            if (instructor == null) return NotFound();
            return View(instructor);
        }

        // GET: /Instructors/Create
        public IActionResult Create()
        {
            ViewBag.DepartmentId = new SelectList(
                _context.Departments, "DepartmentId", "Name");
            return View();
        }

        // POST: /Instructors/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Name,DepartmentId")] Instructor instructor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(instructor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.DepartmentId = new SelectList(
                _context.Departments, "DepartmentId", "Name",
                instructor.DepartmentId);
            return View(instructor);
        }

        // GET: /Instructors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var instructor = await _context.Instructors.FindAsync(id);
            if (instructor == null) return NotFound();

            ViewBag.DepartmentId = new SelectList(
                _context.Departments, "DepartmentId", "Name",
                instructor.DepartmentId);
            return View(instructor);
        }

        // POST: /Instructors/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("InstructorId,Name,DepartmentId")] Instructor instructor)
        {
            if (id != instructor.InstructorId) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(instructor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.DepartmentId = new SelectList(
                _context.Departments, "DepartmentId", "Name",
                instructor.DepartmentId);
            return View(instructor);
        }

        // GET: /Instructors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var instructor = await _context.Instructors
                .Include(i => i.Department)
                .Include(i => i.Courses)
                .FirstOrDefaultAsync(i => i.InstructorId == id);

            if (instructor == null) return NotFound();
            return View(instructor);
        }

        // POST: /Instructors/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var instructor = await _context.Instructors.FindAsync(id);
            if (instructor != null)
            {
                _context.Instructors.Remove(instructor);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}