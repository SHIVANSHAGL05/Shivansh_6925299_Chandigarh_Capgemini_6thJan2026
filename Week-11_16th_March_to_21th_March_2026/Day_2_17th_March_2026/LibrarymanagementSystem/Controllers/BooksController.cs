using LibraryApp.Models;
using LibraryApp.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApp.Controllers
{
    public class BooksController : Controller
    {
        // In-memory store (replace with DbContext in production)
        private static List<Book> _books = new List<Book>
        {
            new Book { Id = 1, Title = "Clean Code",              Author = "Robert C. Martin",    PublishedYear = 2008, Genre = "Technology" },
            new Book { Id = 2, Title = "The Great Gatsby",        Author = "F. Scott Fitzgerald", PublishedYear = 1925, Genre = "Fiction"    },
            new Book { Id = 3, Title = "A Brief History of Time", Author = "Stephen Hawking",     PublishedYear = 1988, Genre = "Science"    },
        };

        // ── GET /Books/Index ──────────────────────────────────────────────
        public IActionResult Index()
        {
            // Task 2: ViewBag (welcome message) + ViewData (total count)
            ViewBag.WelcomeMessage = "Welcome to the City Library Catalogue!";
            ViewData["TotalBooks"]  = _books.Count;

            // Task 3: Build strongly-typed BookViewModel list
            var viewModels = _books.Select((b, i) => new BookViewModel
            {
                Book         = b,
                IsAvailable  = i % 2 == 0,                       // alternating for demo
                BorrowerName = i % 2 != 0 ? "Jane Doe" : null    // Task 6
            }).ToList();

            return View(viewModels);   // strongly typed view
        }

        // ── GET /Books/Create ─────────────────────────────────────────────
        public IActionResult Create()
        {
            return View();
        }

        // ── POST /Books/Create ────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Book book)
        {
            if (!ModelState.IsValid)
                return View(book);   // re-show form with validation errors

            book.Id = _books.Count + 1;
            _books.Add(book);

            TempData["SuccessMessage"] = $"'{book.Title}' was added successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
