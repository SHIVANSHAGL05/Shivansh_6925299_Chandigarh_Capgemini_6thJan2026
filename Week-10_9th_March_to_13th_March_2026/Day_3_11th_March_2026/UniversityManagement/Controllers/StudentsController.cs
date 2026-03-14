using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityManagement.Data;
using UniversityManagement.Models;

public class StudentsController : Controller
{
    private readonly UniversityContext _context;

    public StudentsController(UniversityContext context)
    => _context = context;

    // GET: /Students
    public async Task<IActionResult> Index()
    {
        var students = await _context.Students
        .Include(s => s.Enrollments)          // load enrollments
        .ThenInclude(e => e.Course)        // load course per enrollment
        .OrderBy(s => s.FullName)
        .ToListAsync();

        return View(students);
    }

    // GET: /Students/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var student = await _context.Students
        .Include(s => s.Enrollments)
        .ThenInclude(e => e.Course)
        .FirstOrDefaultAsync(s => s.StudentId == id);

        if (student == null) return NotFound();
        return View(student);
    }

    // GET: /Students/Create
    public IActionResult Create() => View();

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
        return View(student);
    }

    // GET: /Students/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var student = await _context.Students.FindAsync(id);
        if (student == null) return NotFound();
        return View(student);
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
        return View(student);
    }

    // GET: /Students/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var student = await _context.Students
        .FirstOrDefaultAsync(s => s.StudentId == id);
        if (student == null) return NotFound();
        return View(student);
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
}
