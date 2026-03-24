using Microsoft.AspNetCore.Mvc;
using StudentRegistration.Models;

namespace StudentRegistration.Controllers
{
    public class StudentController : Controller
    {
        // In-memory store to simulate a database (for demo purposes)
        private static List<Student> _students = new List<Student>();
        private static int _nextId = 1;

        // ─────────────────────────────────────────────
        // GET: /Student/Register
        // Renders the blank registration form
        // ─────────────────────────────────────────────
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // ─────────────────────────────────────────────
        // POST: /Student/Register
        // Handles form submission with model binding
        // ─────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]               // CSRF protection
        public IActionResult Register(Student student)
        {
            // ModelState.IsValid checks all Data Annotation rules
            if (!ModelState.IsValid)
            {
                // Validation failed — return to form with error messages
                return View(student);
            }

            // Assign a unique ID and persist (in-memory store)
            student.Id = _nextId++;
            _students.Add(student);

            // TempData survives a single redirect and is then cleared automatically
            TempData["SuccessMessage"] = $"🎉 Student \"{student.Name}\" registered successfully!";

            // PRG pattern: redirect to Details to prevent duplicate form submissions
            return RedirectToAction("Details", new { id = student.Id });
        }

        // ─────────────────────────────────────────────
        // GET: /Student/Details/{id}
        // Shows the registered student's details
        // ─────────────────────────────────────────────
        [HttpGet]
        public IActionResult Details(int id)
        {
            var student = _students.FirstOrDefault(s => s.Id == id);

            if (student == null)
            {
                return NotFound($"No student found with ID {id}.");
            }

            return View(student);
        }
    }
}
