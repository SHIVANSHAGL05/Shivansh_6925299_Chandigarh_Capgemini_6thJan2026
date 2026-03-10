
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookCodeFirstDemo.Models
{
    [Table("tblBooks")]
    public class BookModel
    {
        [Key]
        public int BookId { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        public int Price { get; set; }
    }
}