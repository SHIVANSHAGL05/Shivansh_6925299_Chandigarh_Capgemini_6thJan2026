using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(150)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Author is required.")]
        [StringLength(100)]
        public string Author { get; set; }

        [Range(1000, 2100, ErrorMessage = "Please enter a valid year.")]
        public int PublishedYear { get; set; }

        [Required(ErrorMessage = "Genre is required.")]
        public string Genre { get; set; }
    }
}
