using Microsoft.EntityFrameworkCore;

namespace BookCodeFirstDemo.Models
{
    public class BookDBContext : DbContext
    {
        public BookDBContext(DbContextOptions<BookDBContext> options)
            : base(options)
        {
        }

        public DbSet<BookModel> books { get; set; }
    }
}