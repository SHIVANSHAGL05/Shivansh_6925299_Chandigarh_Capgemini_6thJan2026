using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UniversityManagement.Data;
using UniversityManagement.Models;

public class CoursesController : Controller
{
    private readonly UniversityContext _context;

    public CoursesController(UniversityContext context)
        => _context = context;

    // GET: /Courses
    public async Task<IActionResult> Index()
    {
        var courses = await _context.Courses
            .Include(c => c.Instructor)
            .ThenInclude(i => i!.Department)
            .Include(c => c.Enrollments)
            .OrderBy(c => c.Title)
            .ToListAsync();

        return View(courses);
    }

    // GET: /Courses/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var course = await _context.Courses
            .Include(c => c.Instructor)
            .ThenInclude(i => i!.Department)
            .Include(c => c.Enrollments)
            .ThenInclude(e => e.Student)
            .FirstOrDefaultAsync(c => c.CourseId == id);

        if (course == null) return NotFound();

        return View(course);
    }

    // GET: /Courses/Create
    public IActionResult Create()
    {
        ViewBag.InstructorId = new SelectList(
            _context.Instructors,
            "InstructorId",
            "Name");

        return View();
    }

    // POST: /Courses/Create
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("Title,Credits,InstructorId")] Course course)
    {
        if (ModelState.IsValid)
        {
            _context.Add(course);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewBag.InstructorId = new SelectList(
            _context.Instructors,
            "InstructorId",
            "Name",
            course.InstructorId);

        return View(course);
    }

    // GET: /Courses/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var course = await _context.Courses.FindAsync(id);
        if (course == null) return NotFound();

        ViewBag.InstructorId = new SelectList(
            _context.Instructors,
            "InstructorId",
            "Name",
            course.InstructorId);

        return View(course);
    }

    // POST: /Courses/Edit/5
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id,
        [Bind("CourseId,Title,Credits,InstructorId")] Course course)
    {
        if (id != course.CourseId) return NotFound();

        if (ModelState.IsValid)
        {
            _context.Update(course);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewBag.InstructorId = new SelectList(
            _context.Instructors,
            "InstructorId",
            "Name",
            course.InstructorId);

        return View(course);
    }

    // GET: /Courses/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var course = await _context.Courses
            .Include(c => c.Instructor)
            .FirstOrDefaultAsync(c => c.CourseId == id);

        if (course == null) return NotFound();

        return View(course);
    }

    // POST: /Courses/Delete/5
    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var course = await _context.Courses.FindAsync(id);

        if (course != null)
        {
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}