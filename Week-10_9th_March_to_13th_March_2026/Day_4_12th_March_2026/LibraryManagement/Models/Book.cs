using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagement.Models
{
    public class Book
    {
        public int BookId { get; set; }

        [Required, StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string Author { get; set; } = string.Empty;

        [Required, StringLength(20)]
        public string ISBN { get; set; } = string.Empty;

        [StringLength(100)]
        public string Genre { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [Display(Name = "Published Date")]
        public DateTime PublishedDate { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        [Display(Name = "Available")]
        public bool IsAvailable { get; set; } = true;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
    }
}
