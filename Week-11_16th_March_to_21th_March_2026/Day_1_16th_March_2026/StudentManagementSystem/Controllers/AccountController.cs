using Microsoft.AspNetCore.Mvc;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Controllers
{
    /// <summary>
    /// AccountController manages session-based authentication.
    ///
    /// Session key used: "Username"
    ///   - Set on successful login  : HttpContext.Session.SetString("Username", value)
    ///   - Read to verify login     : HttpContext.Session.GetString("Username")
    ///   - Cleared on logout        : HttpContext.Session.Clear()
    /// </summary>
    public class AccountController : Controller
    {
        private const string SessionKeyUsername = "Username";

        // ── GET /Account/Login ────────────────────────────────────────────────
        [HttpGet]
        public IActionResult Login()
        {
            // Already logged in → go straight to dashboard.
            if (HttpContext.Session.GetString(SessionKeyUsername) != null)
                return RedirectToAction("Index", "Student");

            return View(new LoginViewModel());
        }

        // ── POST /Account/Login ───────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Hard-coded credential check.
            // In production: hash the password and compare against a DB record.
            if (model.Username == "admin" && model.Password == "123")
            {
                HttpContext.Session.SetString(SessionKeyUsername, model.Username);
                TempData["SuccessMessage"] = $"Welcome back, {model.Username}!";
                return RedirectToAction("Index", "Student");
            }

            ViewBag.ErrorMessage = "Invalid username or password. Try admin / 123.";
            return View(model);
        }

        // ── GET /Account/Logout ───────────────────────────────────────────────
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["InfoMessage"] = "You have been logged out successfully.";
            return RedirectToAction("Login");
        }
    }
}
