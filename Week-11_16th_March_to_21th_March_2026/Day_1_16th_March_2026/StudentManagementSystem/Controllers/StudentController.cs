using Microsoft.AspNetCore.Mvc;
using StudentManagementSystem.Data;
using StudentManagementSystem.Filters;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Controllers
{
    /// <summary>
    /// StudentController handles:
    ///   Index    — list all students
    ///   Register — GET: show form / POST: validate, save, redirect
    ///   Details  — show one student's info + TempData success message
    ///
    /// [TypeFilter(typeof(SessionAuthFilter))] on the class means
    /// EVERY action here requires a valid session (user must be logged in).
    ///
    /// [TypeFilter(typeof(LogActionFilter))] logs every action automatically.
    /// </summary>
    [TypeFilter(typeof(SessionAuthFilter))]
    [TypeFilter(typeof(LogActionFilter))]
    public class StudentController : Controller
    {
        // StudentRepository is injected via DI (registered as Singleton in Program.cs).
        private readonly StudentRepository _repo;

        public StudentController(StudentRepository repo)
        {
            _repo = repo;
        }

        // ── GET /Student/Index ────────────────────────────────────────────────
        // Lists all registered students with a link to register a new one.
        [HttpGet]
        public IActionResult Index()
        {
            var students = _repo.GetAll();
            ViewBag.Username = HttpContext.Session.GetString("Username");
            return View(students);
        }

        // ── GET /Student/Register ─────────────────────────────────────────────
        // Returns a blank registration form.
        [HttpGet]
        public IActionResult Register()
        {
            return View(new Student());
        }

        // ── POST /Student/Register ────────────────────────────────────────────
        //
        // MODEL BINDING: ASP.NET Core maps form fields → Student object
        //   automatically by matching HTML input name attributes to property names.
        //
        // VALIDATION:
        //   ModelState.IsValid checks all Data Annotations on Student.
        //   If any rule fails, ModelState is false and we return the form
        //   with inline error messages.
        //
        // TEMPDATA:
        //   Survives exactly one redirect. Used to show the success banner
        //   on the Details page after the PRG redirect.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(Student student)
        {
            if (!ModelState.IsValid)
            {
                // Validation failed — return form with error messages.
                return View(student);
            }

            // Save to in-memory repository (student.Id is assigned here).
            var saved = _repo.Add(student);

            // TempData persists across exactly one redirect.
            TempData["SuccessMessage"] =
                $"Student \"{saved.Name}\" registered successfully with ID #{saved.Id}!";

            // PRG: redirect to Details to prevent form re-submission on refresh.
            return RedirectToAction("Details", new { id = saved.Id });
        }

        // ── GET /Student/Details/{id} ─────────────────────────────────────────
        // Shows the registered student's full details.
        // TempData["SuccessMessage"] is read and displayed here.
        [HttpGet]
        public IActionResult Details(int id)
        {
            var student = _repo.GetById(id);

            if (student == null)
                return NotFound($"No student found with ID {id}.");

            return View(student);
        }
    }
}
