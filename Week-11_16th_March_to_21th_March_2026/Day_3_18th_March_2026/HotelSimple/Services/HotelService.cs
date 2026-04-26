using HotelSimple.Models;

namespace HotelSimple.Services
{
    public class HotelService
    {
        // Static lists act as our "database"
        private static List<Room> _rooms = new List<Room>
        {
            new Room { Id = 1, RoomNumber = "101", RoomType = "Standard", PricePerNight = 2500, Capacity = 2, Description = "Cozy standard room with garden view, TV, and AC.", IsAvailable = true },
            new Room { Id = 2, RoomNumber = "102", RoomType = "Standard", PricePerNight = 2500, Capacity = 2, Description = "Comfortable standard room on ground floor.", IsAvailable = true },
            new Room { Id = 3, RoomNumber = "201", RoomType = "Deluxe",   PricePerNight = 5000, Capacity = 3, Description = "Spacious deluxe room with city view and mini bar.", IsAvailable = true },
            new Room { Id = 4, RoomNumber = "202", RoomType = "Deluxe",   PricePerNight = 5000, Capacity = 3, Description = "Deluxe room with king-size bed and bathtub.", IsAvailable = true },
            new Room { Id = 5, RoomNumber = "301", RoomType = "Suite",    PricePerNight = 9500, Capacity = 4, Description = "Luxury suite with living area, jacuzzi, and panoramic view.", IsAvailable = true },
        };

        private static List<Customer> _customers = new List<Customer>
        {
            new Customer { Id = 1, FullName = "Rahul Sharma",  Email = "rahul@example.com",  Phone = "9876543210" },
            new Customer { Id = 2, FullName = "Priya Mehta",   Email = "priya@example.com",  Phone = "9123456789" },
        };

        private static List<Booking> _bookings = new List<Booking>();

        private static int _nextCustomerId = 3;
        private static int _nextBookingId  = 1;

        static HotelService()
        {
            // Add one sample booking so admin dashboard is not empty
            var b = new Booking
            {
                Id         = _nextBookingId++,
                BookingRef = "HB-001",
                RoomId     = 3,
                CustomerId = 1,
                CheckIn    = DateTime.Today.AddDays(-1),
                CheckOut   = DateTime.Today.AddDays(2),
                Guests     = 2,
                Status     = "Confirmed",
                TotalAmount = 15000,
                BookedOn   = DateTime.Now.AddDays(-2),
                Room       = _rooms[2],
                Customer   = _customers[0]
            };
            _bookings.Add(b);
        }

        // ── Rooms ───────────────────────────────────────────────

        public List<Room> GetAllRooms() => _rooms;

        public Room? GetRoom(int id) => _rooms.FirstOrDefault(r => r.Id == id);

        // Returns rooms not booked for the given dates
        public List<Room> GetAvailableRooms(DateTime checkIn, DateTime checkOut)
        {
            // Find room IDs that have overlapping bookings
            var bookedRoomIds = _bookings
                .Where(b => b.Status != "Cancelled" &&
                            b.CheckIn  < checkOut &&
                            b.CheckOut > checkIn)
                .Select(b => b.RoomId)
                .ToList();

            return _rooms
                .Where(r => !bookedRoomIds.Contains(r.Id))
                .ToList();
        }

        public bool IsRoomAvailable(int roomId, DateTime checkIn, DateTime checkOut)
        {
            return !_bookings.Any(b =>
                b.RoomId   == roomId &&
                b.Status   != "Cancelled" &&
                b.CheckIn  < checkOut &&
                b.CheckOut > checkIn);
        }

        // ── Customers ────────────────────────────────────────────

        public Customer? GetCustomerByEmail(string email)
            => _customers.FirstOrDefault(c =>
                c.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

        public Customer AddCustomer(Customer c)
        {
            c.Id = _nextCustomerId++;
            _customers.Add(c);
            return c;
        }

        // ── Bookings ─────────────────────────────────────────────

        public List<Booking> GetAllBookings()
            => _bookings.OrderByDescending(b => b.BookedOn).ToList();

        public Booking? GetBooking(int id)
            => _bookings.FirstOrDefault(b => b.Id == id);

        public Booking? GetBookingByRef(string bookingRef)
            => _bookings.FirstOrDefault(b =>
                b.BookingRef.Equals(bookingRef, StringComparison.OrdinalIgnoreCase));

        public Booking CreateBooking(Booking b)
        {
            b.Id         = _nextBookingId;
            b.BookingRef = "HB-" + _nextBookingId.ToString("D3");
            _nextBookingId++;

            b.Room     = _rooms.First(r => r.Id == b.RoomId);
            b.Customer = _customers.First(c => c.Id == b.CustomerId);

            int nights      = (b.CheckOut - b.CheckIn).Days;
            b.TotalAmount   = nights * b.Room.PricePerNight;
            b.BookedOn      = DateTime.Now;

            _bookings.Add(b);
            return b;
        }

        public bool CancelBooking(int id)
        {
            var booking = _bookings.FirstOrDefault(b => b.Id == id);
            if (booking == null) return false;
            booking.Status = "Cancelled";
            return true;
        }
    }
}
