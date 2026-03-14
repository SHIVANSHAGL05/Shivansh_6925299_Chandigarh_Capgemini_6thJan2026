using Microsoft.EntityFrameworkCore;
using LibraryManagement.Models;

namespace LibraryManagement.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    "Server=Blackpearl\\SQLEXPRESS;Database=LibraryManagementDb;Trusted_Connection=True;TrustServerCertificate=True");
            }
        }

        public DbSet<Book> Books { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed data
            modelBuilder.Entity<Book>().HasData(
                new Book
                {
                    BookId = 1, Title = "Clean Code", Author = "Robert C. Martin",
                    ISBN = "978-0132350884", Genre = "Programming",
                    PublishedDate = new DateTime(2008, 8, 1),
                    Price = 35.99m, IsAvailable = true,
                    Description = "A handbook of agile software craftsmanship."
                },
                new Book
                {
                    BookId = 2, Title = "The Pragmatic Programmer", Author = "Andrew Hunt",
                    ISBN = "978-0135957059", Genre = "Programming",
                    PublishedDate = new DateTime(2019, 9, 1),
                    Price = 49.99m, IsAvailable = true,
                    Description = "Your journey to mastery."
                },
                new Book
                {
                    BookId = 3, Title = "Design Patterns", Author = "Gang of Four",
                    ISBN = "978-0201633610", Genre = "Software Architecture",
                    PublishedDate = new DateTime(1994, 10, 1),
                    Price = 54.99m, IsAvailable = false,
                    Description = "Elements of reusable object-oriented software."
                },
                new Book
                {
                    BookId = 4, Title = "C# in Depth", Author = "Jon Skeet",
                    ISBN = "978-1617294532", Genre = "Programming",
                    PublishedDate = new DateTime(2019, 3, 1),
                    Price = 44.99m, IsAvailable = true,
                    Description = "A deep dive into the C# language."
                },
                new Book
                {
                    BookId = 5, Title = "ASP.NET Core in Action", Author = "Andrew Lock",
                    ISBN = "978-1617298301", Genre = "Web Development",
                    PublishedDate = new DateTime(2021, 6, 1),
                    Price = 59.99m, IsAvailable = true,
                    Description = "Build cross-platform web apps with ASP.NET Core."
                }
            );
        }
    }
}
