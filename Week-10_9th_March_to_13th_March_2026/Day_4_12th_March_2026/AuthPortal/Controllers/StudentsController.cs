// Controllers/StudentsController.cs
using Microsoft.AspNetCore.Mvc;
using AuthPortal.Models;
using AuthPortal.Services;

namespace AuthPortal.Controllers;

/// <summary>
/// Public controller — no authentication required.
/// All /Students/* routes are accessible by anyone.
/// Students can only VIEW the list here (read-only public section).
/// </summary>
public class StudentsController : Controller
{
    private readonly IStudentService _studentService;

    public StudentsController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    // GET /Students/Index  — public page
    public IActionResult Index()
    {
        var viewModel = new StudentIndexViewModel
        {
            Students = _studentService.GetAll()
        };
        return View(viewModel);
    }
}