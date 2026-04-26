using Microsoft.AspNetCore.Mvc;
using EmployeePortal.Data;
using EmployeePortal.Filters;
using EmployeePortal.Models;

namespace EmployeePortal.Controllers
{
    /// <summary>
    /// HRController — HR Productivity Module.
    ///
    /// Demonstrates:
    ///   1. LogActionFilter — logs controller, action, user, timestamp for every action.
    ///   2. SessionAuthFilter — all HR pages require admin login.
    ///   3. CustomExceptionFilter (global) — SimulateReportsError throws to prove it works.
    ///
    /// Actions:
    ///   Index              — HR home / summary dashboard
    ///   EmployeeList       — full employee list with department filter
    ///   Reports            — department statistics report
    ///   SimulateReportsError — throws intentionally to test exception filter
    /// </summary>
    [TypeFilter(typeof(SessionAuthFilter))]
    [TypeFilter(typeof(LogActionFilter))]
    public class HRController : Controller
    {
        private readonly EmployeeRepository _repo;
        private readonly ILogger<HRController> _logger;

        public HRController(EmployeeRepository repo, ILogger<HRController> logger)
        {
            _repo   = repo;
            _logger = logger;
        }

        // ── GET /HR/Index ─────────────────────────────────────────────────────
        [HttpGet]
        public IActionResult Index()
        {
            _logger.LogInformation("[HRController] Index — loading HR summary.");
            ViewBag.Username      = HttpContext.Session.GetString("Username");
            ViewBag.TotalEmployees = _repo.Count();
            ViewBag.Departments   = _repo.GetDepartments().Count();
            ViewBag.NewThisMonth  = _repo.GetAll()
                .Count(e => e.JoinedOn.Month == DateTime.Now.Month
                         && e.JoinedOn.Year  == DateTime.Now.Year);
            return View();
        }

        // ── GET /HR/EmployeeList ──────────────────────────────────────────────
        [HttpGet]
        public IActionResult EmployeeList(string? department)
        {
            _logger.LogInformation("[HRController] EmployeeList — dept filter: {Dept}", department ?? "All");
            ViewBag.Username    = HttpContext.Session.GetString("Username");
            ViewBag.Departments = _repo.GetDepartments();
            ViewBag.Selected    = department;

            var list = string.IsNullOrEmpty(department)
                ? _repo.GetAll()
                : _repo.GetByDept(department);

            return View(list);
        }

        // ── GET /HR/Reports ───────────────────────────────────────────────────
        [HttpGet]
        public IActionResult Reports()
        {
            _logger.LogInformation("[HRController] Reports — generating department stats.");
            ViewBag.Username = HttpContext.Session.GetString("Username");

            // Build department-wise summary for the report view.
            var report = _repo.GetAll()
                .GroupBy(e => e.Department)
                .Select(g => new DepartmentReport
                {
                    Department = g.Key ?? "Unknown",
                    Count      = g.Count(),
                    AvgAge     = (int)g.Average(e => e.Age),
                    Newest     = g.Max(e => e.JoinedOn).ToString("dd MMM yyyy")
                })
                .OrderByDescending(r => r.Count)
                .ToList();

            return View(report);
        }

        // ── GET /HR/SimulateReportsError ──────────────────────────────────────
        // Deliberately throws an exception.
        // CustomExceptionFilter (registered globally) catches it and renders
        // Views/Shared/Error.cshtml instead of crashing.
        [HttpGet]
        public IActionResult SimulateReportsError()
        {
            _logger.LogInformation("[HRController] SimulateReportsError — about to throw.");

            throw new InvalidOperationException(
                "Simulated Reports module failure — this exception is caught by " +
                "CustomExceptionFilter and rendered as a friendly error page.");
        }
    }

    // ── Department report ViewModel ───────────────────────────────────────────
    /// <summary>
    /// Used only by HRController.Reports to pass grouped data to the view.
    /// Not stored in the database — purely for display.
    /// </summary>
    public class DepartmentReport
    {
        public string Department { get; set; } = string.Empty;
        public int    Count      { get; set; }
        public int    AvgAge     { get; set; }
        public string Newest     { get; set; } = string.Empty;
    }
}
