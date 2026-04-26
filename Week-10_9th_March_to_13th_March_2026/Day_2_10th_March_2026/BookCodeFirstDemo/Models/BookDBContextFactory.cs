using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BookCodeFirstDemo.Models
{
    public class BookDBContextFactory : IDesignTimeDbContextFactory<BookDBContext>
    {
        public BookDBContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<BookDBContext>();

            optionsBuilder.UseSqlServer(
                "Server=Blackpearl\\SQLEXPRESS;Database=BookDB;Integrated Security=True;TrustServerCertificate=True;");

            return new BookDBContext(optionsBuilder.Options);
        }
    }
}