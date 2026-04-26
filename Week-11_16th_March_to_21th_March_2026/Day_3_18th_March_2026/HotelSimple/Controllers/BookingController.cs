using HotelSimple.Models;
using HotelSimple.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelSimple.Controllers
{
    public class BookingController : Controller
    {
        private readonly HotelService _service;

        public BookingController(HotelService service)
        {
            _service = service;
        }

        // Step 1 – Show available rooms for chosen dates
        // GET /Booking?checkIn=2025-01-01&checkOut=2025-01-03
        public IActionResult Index(string? checkIn, string? checkOut)
        {
            // If dates not provided, just show all rooms
            if (string.IsNullOrEmpty(checkIn) || string.IsNullOrEmpty(checkOut))
            {
                ViewBag.Rooms    = _service.GetAllRooms();
                ViewBag.CheckIn  = DateTime.Today.ToString("yyyy-MM-dd");
                ViewBag.CheckOut = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");
                ViewBag.Searched = false;
                return View();
            }

            var ci = DateTime.Parse(checkIn);
            var co = DateTime.Parse(checkOut);

            if (co <= ci)
            {
                TempData["Error"] = "Check-out date must be after check-in date.";
                ViewBag.Rooms    = _service.GetAllRooms();
                ViewBag.CheckIn  = checkIn;
                ViewBag.CheckOut = checkOut;
                ViewBag.Searched = false;
                return View();
            }

            ViewBag.Rooms    = _service.GetAvailableRooms(ci, co);
            ViewBag.CheckIn  = checkIn;
            ViewBag.CheckOut = checkOut;
            ViewBag.Nights   = (co - ci).Days;
            ViewBag.Searched = true;
            return View();
        }

        // Step 2 – Show booking form for a selected room
        // GET /Booking/Book?roomId=3&checkIn=...&checkOut=...
        public IActionResult Book(int roomId, string checkIn, string checkOut)
        {
            var room = _service.GetRoom(roomId);
            if (room == null) return NotFound();

            var ci = DateTime.Parse(checkIn);
            var co = DateTime.Parse(checkOut);

            // Check availability one more time before showing form
            if (!_service.IsRoomAvailable(roomId, ci, co))
            {
                TempData["Error"] = "Sorry, this room is no longer available for those dates.";
                return RedirectToAction("Index", new { checkIn, checkOut });
            }

            ViewBag.Room     = room;
            ViewBag.CheckIn  = checkIn;
            ViewBag.CheckOut = checkOut;
            ViewBag.Nights   = (co - ci).Days;
            ViewBag.Total    = (co - ci).Days * room.PricePerNight;

            return View();
        }

        // Step 3 – Handle form submission and create booking
        // POST /Booking/Book
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Book(string fullName, string email, string phone,
                                  int roomId, string checkIn, string checkOut, int guests)
        {
            // Basic validation
            if (string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(phone))
            {
                TempData["Error"] = "Please fill in all required fields.";
                return RedirectToAction("Book", new { roomId, checkIn, checkOut });
            }

            var ci = DateTime.Parse(checkIn);
            var co = DateTime.Parse(checkOut);

            // Get or create customer
            var customer = _service.GetCustomerByEmail(email);
            if (customer == null)
            {
                customer = _service.AddCustomer(new Customer
                {
                    FullName = fullName,
                    Email    = email,
                    Phone    = phone
                });
            }

            // Create the booking
            var booking = _service.CreateBooking(new Booking
            {
                RoomId     = roomId,
                CustomerId = customer.Id,
                CheckIn    = ci,
                CheckOut   = co,
                Guests     = guests,
                Status     = "Confirmed"
            });

            return RedirectToAction("Confirmation", new { id = booking.Id });
        }

        // Step 4 – Show confirmation page
        // GET /Booking/Confirmation/5
        public IActionResult Confirmation(int id)
        {
            var booking = _service.GetBooking(id);
            if (booking == null) return NotFound();
            return View(booking);
        }

        // GET /Booking/Lookup
        public IActionResult Lookup(string? bookingRef)
        {
            if (string.IsNullOrWhiteSpace(bookingRef))
                return View((Booking?)null);

            var booking = _service.GetBookingByRef(bookingRef.Trim().ToUpper());
            if (booking == null)
                TempData["Error"] = "No booking found with that reference number.";

            return View(booking);
        }

        // POST /Booking/Cancel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cancel(int id)
        {
            _service.CancelBooking(id);
            TempData["Success"] = "Your booking has been cancelled.";
            return RedirectToAction("Lookup");
        }
    }
}
