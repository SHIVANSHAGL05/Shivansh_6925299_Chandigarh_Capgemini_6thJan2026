using Microsoft.AspNetCore.Mvc;
using EmployeePortal.Data;
using EmployeePortal.Filters;
using EmployeePortal.Models;

namespace EmployeePortal.Controllers
{
    /// <summary>
    /// EmployeeController — handles employee self-registration.
    ///
    /// Protected by SessionAuthFilter (must be logged in).
    /// Logged by LogActionFilter (every action is recorded).
    ///
    /// Actions:
    ///   Index    — list all employees
    ///   Register — GET: show form | POST: validate, save, redirect
    ///   Details  — show one employee's info + TempData success banner
    /// </summary>
    [TypeFilter(typeof(SessionAuthFilter))]
    [TypeFilter(typeof(LogActionFilter))]
    public class EmployeeController : Controller
    {
        private readonly EmployeeRepository _repo;
        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(EmployeeRepository repo, ILogger<EmployeeController> logger)
        {
            _repo   = repo;
            _logger = logger;
        }

        // ── GET /Employee/Index ───────────────────────────────────────────────
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Username = HttpContext.Session.GetString("Username");
            var employees = _repo.GetAll();
            return View(employees);
        }

        // ── GET /Employee/Register ────────────────────────────────────────────
        [HttpGet]
        public IActionResult Register()
        {
            return View(new Employee());
        }

        // ── POST /Employee/Register ───────────────────────────────────────────
        // Model Binding: ASP.NET Core maps form fields → Employee object
        // automatically by matching HTML input name attributes to property names.
        //
        // Validation: ModelState.IsValid checks all Data Annotations.
        // If any rule fails → return form with inline error messages.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(Employee employee)
        {
            if (!ModelState.IsValid)
                return View(employee);

            var saved = _repo.Add(employee);

            // TempData survives ONE redirect then auto-clears.
            TempData["SuccessMessage"] =
                $"Employee \"{saved.Name}\" registered successfully with ID #{saved.Id}!";

            // PRG pattern — redirect prevents form re-submission on F5.
            return RedirectToAction("Details", new { id = saved.Id });
        }

        // ── GET /Employee/Details/{id} ────────────────────────────────────────
        [HttpGet]
        public IActionResult Details(int id)
        {
            var employee = _repo.GetById(id);
            if (employee == null)
                return NotFound($"No employee found with ID {id}.");

            return View(employee);
        }
    }
}
