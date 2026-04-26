using Microsoft.AspNetCore.Mvc;

namespace SessionLogin.Controllers
{
    /// <summary>
    /// AccountController handles three responsibilities:
    ///   1. Login  (GET + POST) — validates credentials, writes to Session
    ///   2. Dashboard (GET)    — reads Session to show the logged-in username
    ///   3. Logout (GET)       — clears Session, redirects to Login
    ///
    /// HOW SESSION WORKS IN ASP.NET CORE:
    ///   - Session data is stored SERVER-SIDE (in memory by default).
    ///   - The browser receives a cookie named ".AspNetCore.Session"
    ///     containing only the SESSION ID (not the actual data).
    ///   - On every request, ASP.NET Core reads the cookie → looks up the
    ///     matching server-side dictionary → makes it available via HttpContext.Session.
    ///   - Session must be registered in Program.cs:
    ///       builder.Services.AddSession(...)   ← registers the service
    ///       app.UseSession()                    ← adds it to the middleware pipeline
    /// </summary>
    public class AccountController : Controller
    {
        // ─────────────────────────────────────────────────────────────────────
        // SESSION KEY CONSTANT
        // Using a constant prevents typos when reading/writing the same key.
        // ─────────────────────────────────────────────────────────────────────
        private const string SessionKeyUsername = "Username";

        // ─────────────────────────────────────────────────────────────────────
        // GET: /Account/Login
        // Simply renders the blank login form.
        // ─────────────────────────────────────────────────────────────────────
        [HttpGet]
        public IActionResult Login()
        {
            // If user is already logged in, send them straight to Dashboard.
            if (HttpContext.Session.GetString(SessionKeyUsername) != null)
            {
                return RedirectToAction("Dashboard");
            }

            return View();
        }

        // ─────────────────────────────────────────────────────────────────────
        // POST: /Account/Login
        //
        // Model Binding maps the form fields to the parameters automatically.
        //
        // VALIDATION FLOW:
        //   1. Check credentials against hard-coded values.
        //   2. If valid   → store username in Session → redirect to Dashboard.
        //   3. If invalid → add a ModelState error → return View (shows error).
        // ─────────────────────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string username, string password)
        {
            // Hard-coded credential check (replace with DB lookup in production).
            if (username == "admin" && password == "123")
            {
                // ── Write to Session ─────────────────────────────────────────
                // HttpContext.Session.SetString(key, value) serialises the value
                // and stores it in the server-side session dictionary.
                // The session ID cookie is automatically sent to the browser.
                HttpContext.Session.SetString(SessionKeyUsername, username);

                // PRG pattern: redirect after POST so refresh doesn't re-submit.
                return RedirectToAction("Dashboard");
            }

            // Invalid credentials — add error and re-display form.
            // ViewBag is used here to pass a simple one-off message to the view.
            ViewBag.ErrorMessage = "Invalid username or password. Please try again.";
            return View();
        }

        // ─────────────────────────────────────────────────────────────────────
        // GET: /Account/Dashboard
        //
        // GUARD CHECK (Requirement 4):
        //   Read Session — if the username key is absent, the user is not
        //   logged in, so redirect them back to the Login page.
        // ─────────────────────────────────────────────────────────────────────
        [HttpGet]
        public IActionResult Dashboard()
        {
            // HttpContext.Session.GetString returns null if the key doesn't exist.
            var username = HttpContext.Session.GetString(SessionKeyUsername);

            if (username == null)
            {
                // Not logged in → redirect to Login.
                // TempData message tells the Login view WHY they were redirected.
                TempData["InfoMessage"] = "Please log in to access the dashboard.";
                return RedirectToAction("Login");
            }

            // Pass username to the view via ViewBag.
            ViewBag.Username = username;
            return View();
        }

        // ─────────────────────────────────────────────────────────────────────
        // GET: /Account/Logout
        //
        // HttpContext.Session.Clear() removes ALL keys from the session dictionary.
        // The session ID cookie on the browser becomes useless (no matching data).
        // ─────────────────────────────────────────────────────────────────────
        [HttpGet]
        public IActionResult Logout()
        {
            // Clear every key stored in this session.
            HttpContext.Session.Clear();

            // Optionally: HttpContext.Session.Remove(SessionKeyUsername)
            // removes only a specific key — useful if you store multiple values.

            TempData["InfoMessage"] = "You have been logged out successfully.";
            return RedirectToAction("Login");
        }
    }
}
