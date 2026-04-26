using System.ComponentModel.DataAnnotations;

namespace HotelSimple.Models
{
    public class Room
    {
        public int Id { get; set; }

        [Required]
        public string RoomNumber { get; set; } = "";

        [Required]
        public string RoomType { get; set; } = ""; // Standard, Deluxe, Suite

        [Required]
        [Range(500, 50000)]
        public decimal PricePerNight { get; set; }

        [Required]
        public int Capacity { get; set; }

        public string Description { get; set; } = "";

        public bool IsAvailable { get; set; } = true;
    }
}
