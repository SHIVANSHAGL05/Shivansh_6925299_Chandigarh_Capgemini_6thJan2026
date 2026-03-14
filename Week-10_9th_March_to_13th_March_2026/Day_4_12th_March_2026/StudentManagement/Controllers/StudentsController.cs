using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Data;
using StudentPortal.Services;
using StudentPortal.ViewModels;
using StudentPortal.Models;

namespace StudentPortal.Controllers
{
    public class StudentsController : Controller
    {
        private readonly AppDbContext        _context;
        private readonly IRequestLogService  _logService;

        // Both dependencies injected via constructor DI
        public StudentsController(
            AppDbContext       context,
            IRequestLogService logService)
        {
            _context    = context;
            _logService = logService;
        }

        // GET: /Students/Index
        public async Task<IActionResult> Index()
        {
            var viewModel = new StudentIndexViewModel
            {
                Students    = await _context.Students
                                            .OrderBy(s => s.FullName)
                                            .ToListAsync(),
                RequestLogs = _logService.GetAll()
                                         .OrderByDescending(l => l.Timestamp)
                                         .ToList()
            };

            // View is in Views/StudentManagement/ folder as per requirement
            return View("~/Views/StudentManagement/Index.cshtml", viewModel);
        }

        // GET: /Students/Create
        public IActionResult Create() =>
            View("~/Views/StudentManagement/Create.cshtml");

        // POST: /Students/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("FullName,Email,EnrollmentDate")] Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/StudentManagement/Create.cshtml", student);
        }

        // GET: /Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound();
            return View("~/Views/StudentManagement/Edit.cshtml", student);
        }

        // POST: /Students/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("StudentId,FullName,Email,EnrollmentDate")] Student student)
        {
            if (id != student.StudentId) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/StudentManagement/Edit.cshtml", student);
        }

        // GET: /Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == id);
            if (student == null) return NotFound();
            return View("~/Views/StudentManagement/Delete.cshtml", student);
        }

        // POST: /Students/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: /Students/ClearLogs
        [HttpPost]
        public IActionResult ClearLogs()
        {
            _logService.Clear();
            return RedirectToAction(nameof(Index));
        }
    }
}
