using HotelSimple.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelSimple.Controllers
{
    public class AdminController : Controller
    {
        private readonly HotelService _service;

        public AdminController(HotelService service)
        {
            _service = service;
        }

        // GET /Admin
        public IActionResult Index()
        {
            var bookings = _service.GetAllBookings();
            var rooms    = _service.GetAllRooms();

            ViewBag.TotalRooms      = rooms.Count;
            ViewBag.TotalBookings   = bookings.Count;
            ViewBag.ActiveBookings  = bookings.Count(b => b.Status == "Confirmed");
            ViewBag.TotalRevenue    = bookings.Where(b => b.Status != "Cancelled").Sum(b => b.TotalAmount);
            ViewBag.RecentBookings  = bookings.Take(5).ToList();
            ViewBag.Rooms           = rooms;

            return View();
        }

        // GET /Admin/Bookings
        public IActionResult Bookings()
        {
            var bookings = _service.GetAllBookings();
            return View(bookings);
        }

        // POST /Admin/Cancel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cancel(int id)
        {
            _service.CancelBooking(id);
            TempData["Success"] = "Booking cancelled successfully.";
            return RedirectToAction("Bookings");
        }
    }
}
