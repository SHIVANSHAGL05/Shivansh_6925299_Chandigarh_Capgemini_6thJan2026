using HotelSimple.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelSimple.Controllers
{
    public class HomeController : Controller
    {
        private readonly HotelService _service;

        public HomeController(HotelService service)
        {
            _service = service;
        }

        // GET /
        public IActionResult Index()
        {
            ViewBag.TotalRooms = _service.GetAllRooms().Count;
            return View();
        }

        // POST / – search availability from homepage
        [HttpPost]
        public IActionResult Search(string checkIn, string checkOut)
        {
            return RedirectToAction("Index", "Booking",
                new { checkIn, checkOut });
        }
    }
}
