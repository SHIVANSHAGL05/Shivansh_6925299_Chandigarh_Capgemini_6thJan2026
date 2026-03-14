using Microsoft.AspNetCore.Mvc;

namespace EmployeeProjectManagementSystem.Controllers
{
    public class EmployeeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
