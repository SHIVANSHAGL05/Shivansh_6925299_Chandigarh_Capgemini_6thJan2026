// Controllers/AdminController.cs
using Microsoft.AspNetCore.Mvc;
using AuthPortal.Models;
using AuthPortal.Services;
using System.Diagnostics;

namespace AuthPortal.Controllers;

/// <summary>
/// Protected controller — all /Admin/* routes guarded by AuthenticationMiddleware.
/// Full CRUD on students is only available here after login.
/// IAuthService + IStudentService + IOperationLogService injected via DI.
/// </summary>
public class AdminController : Controller
{
    private readonly IAuthService _authService;
    private readonly IStudentService _studentService;
    private readonly IOperationLogService _logService;

    // Constructor DI — all three services provided by DI container
    public AdminController(
        IAuthService authService,
        IStudentService studentService,
        IOperationLogService logService)
    {
        _authService = authService;
        _studentService = studentService;
        _logService = logService;
    }

    // GET /Admin/Index  — protected dashboard
    public IActionResult Index()
    {
        var viewModel = new AdminDashboardViewModel
        {
            Students = _studentService.GetAll(),
            OperationLogs = _logService.GetLogs(),
            NewStudent = new Student(),
            LoggedInUser = HttpContext.Session.GetString("Username") ?? "Admin"
        };
        return View(viewModel);
    }

    // GET /Admin/Login
    [HttpGet]
    public IActionResult Login()
    {
        // If already logged in redirect to dashboard
        if (_authService.IsAuthenticated(HttpContext))
            return RedirectToAction("Index");

        return View();
    }

    // POST /Admin/Login
    [HttpPost]
    public IActionResult Login(string username, string password)
    {
        if (_authService.ValidateCredentials(username, password))
        {
            _authService.SignIn(HttpContext, username);

            _logService.Log(
                "LOGIN",
                $"User '{username}' logged in successfully",
                "Admin",
                0);

            return RedirectToAction("Index");
        }

        ViewBag.Error = "Invalid username or password.";
        return View();
    }

    // GET /Admin/Logout
    public IActionResult Logout()
    {
        var username = HttpContext.Session.GetString("Username") ?? "Unknown";

        _logService.Log(
            "LOGOUT",
            $"User '{username}' logged out",
            "Admin",
            0);

        _authService.SignOut(HttpContext);
        return RedirectToAction("Index", "Students");
    }

    // POST /Admin/CreateStudent
    [HttpPost]
    public IActionResult CreateStudent(Student student)
    {
        var sw = Stopwatch.StartNew();

        _studentService.Add(student);

        sw.Stop();
        _logService.Log(
            "CREATE",
            $"Added student: '{student.Name}' — {student.Course}",
            "Admin",
            sw.ElapsedMilliseconds);

        return RedirectToAction("Index");
    }

    // POST /Admin/UpdateStudent
    [HttpPost]
    public IActionResult UpdateStudent(Student student)
    {
        var sw = Stopwatch.StartNew();

        _studentService.Update(student);

        sw.Stop();
        _logService.Log(
            "UPDATE",
            $"Updated student ID {student.Id}: '{student.Name}'",
            "Admin",
            sw.ElapsedMilliseconds);

        return RedirectToAction("Index");
    }

    // POST /Admin/DeleteStudent
    [HttpPost]
    public IActionResult DeleteStudent(int id)
    {
        var sw = Stopwatch.StartNew();

        var student = _studentService.GetById(id);
        var name = student?.Name ?? "Unknown";
        _studentService.Delete(id);

        sw.Stop();
        _logService.Log(
            "DELETE",
            $"Deleted student ID {id}: '{name}'",
            "Admin",
            sw.ElapsedMilliseconds);

        return RedirectToAction("Index");
    }
}