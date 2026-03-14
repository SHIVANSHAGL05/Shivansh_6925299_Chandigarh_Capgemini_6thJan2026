using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UniversityManagement.Data;
using UniversityManagement.Models;

namespace UniversityManagement.Controllers
{
    public class EnrollmentsController : Controller
    {
        private readonly UniversityContext _context;

        public EnrollmentsController(UniversityContext context)
            => _context = context;

        // GET: /Enrollments
        public async Task<IActionResult> Index()
        {
            var enrollments = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .OrderBy(e => e.Student.FullName)
                .ToListAsync();

            return View(enrollments);
        }

        // GET: /Enrollments/Create
        public IActionResult Create()
        {
            // Populate dropdowns
            ViewBag.StudentId = new SelectList(_context.Students, "StudentId", "FullName");
            ViewBag.CourseId = new SelectList(_context.Courses, "CourseId", "Title");
            return View();
        }

        // POST: /Enrollments/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("StudentId,CourseId,Grade")] Enrollment enrollment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(enrollment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.StudentId = new SelectList(_context.Students, "StudentId", "FullName", enrollment.StudentId);
            ViewBag.CourseId = new SelectList(_context.Courses, "CourseId", "Title", enrollment.CourseId);
            return View(enrollment);
        }

        // GET: /Enrollments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null) return NotFound();

            ViewBag.StudentId = new SelectList(_context.Students, "StudentId", "FullName", enrollment.StudentId);
            ViewBag.CourseId = new SelectList(_context.Courses, "CourseId", "Title", enrollment.CourseId);
            return View(enrollment);
        }

        // POST: /Enrollments/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("EnrollmentId,StudentId,CourseId,Grade")] Enrollment enrollment)
        {
            if (id != enrollment.EnrollmentId) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(enrollment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.StudentId = new SelectList(_context.Students, "StudentId", "FullName", enrollment.StudentId);
            ViewBag.CourseId = new SelectList(_context.Courses, "CourseId", "Title", enrollment.CourseId);
            return View(enrollment);
        }

        // GET: /Enrollments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var enrollment = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.EnrollmentId == id);

            if (enrollment == null) return NotFound();
            return View(enrollment);
        }

        // POST: /Enrollments/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment != null)
            {
                _context.Enrollments.Remove(enrollment);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}