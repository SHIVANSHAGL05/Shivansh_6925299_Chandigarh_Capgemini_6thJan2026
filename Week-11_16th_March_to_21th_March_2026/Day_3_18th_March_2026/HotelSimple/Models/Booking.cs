using System.ComponentModel.DataAnnotations;

namespace HotelSimple.Models
{
    public class Booking
    {
        public int Id { get; set; }

        public string BookingRef { get; set; } = "";

        // Foreign Keys
        public int RoomId { get; set; }
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Check-in date is required")]
        [DataType(DataType.Date)]
        public DateTime CheckIn { get; set; }

        [Required(ErrorMessage = "Check-out date is required")]
        [DataType(DataType.Date)]
        public DateTime CheckOut { get; set; }

        public int Guests { get; set; } = 1;

        // Status: Pending, Confirmed, Cancelled
        public string Status { get; set; } = "Confirmed";

        public decimal TotalAmount { get; set; }

        public DateTime BookedOn { get; set; } = DateTime.Now;

        // Navigation properties (relationships)
        public Room Room { get; set; } = null!;
        public Customer Customer { get; set; } = null!;

        // Computed helper
        public int Nights => (CheckOut - CheckIn).Days;
    }
}
