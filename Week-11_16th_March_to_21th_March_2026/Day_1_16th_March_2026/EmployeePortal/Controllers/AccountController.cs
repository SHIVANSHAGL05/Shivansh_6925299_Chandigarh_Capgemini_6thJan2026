using Microsoft.AspNetCore.Mvc;
using EmployeePortal.Models;

namespace EmployeePortal.Controllers
{
    /// <summary>
    /// AccountController — session-based admin authentication.
    ///
    /// Session keys used:
    ///   "Username" — stores the logged-in admin's username
    ///   "Role"     — stores the role ("Admin") for future role-based checks
    ///
    /// Credentials: admin / admin123
    /// </summary>
    public class AccountController : Controller
    {
        private const string SessionKeyUsername = "Username";
        private const string SessionKeyRole     = "Role";

        // ── GET /Account/Login ────────────────────────────────────────────────
        [HttpGet]
        public IActionResult Login()
        {
            // Already logged in → redirect to HR dashboard.
            if (HttpContext.Session.GetString(SessionKeyUsername) != null)
                return RedirectToAction("Dashboard");

            return View(new LoginViewModel());
        }

        // ── POST /Account/Login ───────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Validate hard-coded admin credentials.
            // In production: hash password and compare against a database record.
            if (model.Username == "admin" && model.Password == "admin123")
            {
                // Write username AND role to session.
                HttpContext.Session.SetString(SessionKeyUsername, model.Username);
                HttpContext.Session.SetString(SessionKeyRole, "Admin");

                TempData["SuccessMessage"] = $"Welcome back, {model.Username}!";
                return RedirectToAction("Dashboard");
            }

            ViewBag.ErrorMessage = "Invalid credentials. Use admin / admin123.";
            return View(model);
        }

        // ── GET /Account/Dashboard ────────────────────────────────────────────
        [HttpGet]
        public IActionResult Dashboard()
        {
            var username = HttpContext.Session.GetString(SessionKeyUsername);
            var role     = HttpContext.Session.GetString(SessionKeyRole);

            if (username == null)
            {
                TempData["InfoMessage"] = "Please log in to access the dashboard.";
                return RedirectToAction("Login");
            }

            ViewBag.Username = username;
            ViewBag.Role     = role;
            return View();
        }

        // ── GET /Account/Logout ───────────────────────────────────────────────
        [HttpGet]
        public IActionResult Logout()
        {
            // Clear ALL session keys — username, role, anything else stored.
            HttpContext.Session.Clear();
            TempData["InfoMessage"] = "You have been logged out successfully.";
            return RedirectToAction("Login");
        }
    }
}
