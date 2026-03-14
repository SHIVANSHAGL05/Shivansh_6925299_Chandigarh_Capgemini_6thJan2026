using Microsoft.AspNetCore.Mvc;
using LibraryManagement.Repositories;
using LibraryManagement.Models;

namespace LibraryManagement.Controllers
{
    // =====================================================
    // PRACTICE QUESTION 3:
    // Controller using IBookRepository via DI
    // Route: /Books/Details/1 → Details(int id)
    // =====================================================
    // The controller depends on IBookRepository (interface),
    // NOT BookRepository (concrete class).
    // DI injects the correct implementation automatically.
    // =====================================================

    public class BooksController : Controller
    {
        private readonly IBookRepository _bookRepository;

        // DI injects IBookRepository here
        // Because of AddScoped in Program.cs,
        // BookRepository is provided automatically
        public BooksController(IBookRepository bookRepository)
            => _bookRepository = bookRepository;

        // ── GET: /Books/Index ─────────────────────────────
        public async Task<IActionResult> Index(string? search, string? genre)
        {
            IEnumerable<Book> books;

            if (!string.IsNullOrEmpty(search))
                books = await _bookRepository.SearchAsync(search);
            else if (!string.IsNullOrEmpty(genre))
                books = await _bookRepository.GetByGenreAsync(genre);
            else
                books = await _bookRepository.GetAllAsync();

            // Pass search/genre back to view for filter display
            ViewBag.Search = search;
            ViewBag.Genre  = genre;

            return View(books);
        }

        // ── GET: /Books/Details/1 ─────────────────────────
        // PRACTICE QUESTION 3:
        // {id} comes from the conventional route:
        // {controller=Books}/{action=Index}/{id?}
        // URL: /Books/Details/1 → id = 1
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return BadRequest("Book ID is required.");

            var book = await _bookRepository.GetByIdAsync(id.Value);

            if (book == null) return NotFound($"Book with ID {id} was not found.");

            return View(book);
        }

        // ── GET: /Books/Create ────────────────────────────
        public IActionResult Create() => View();

        // ── POST: /Books/Create ───────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Title,Author,ISBN,Genre,PublishedDate,Price,IsAvailable,Description")]
            Book book)
        {
            if (ModelState.IsValid)
            {
                await _bookRepository.AddAsync(book);
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // ── GET: /Books/Edit/1 ────────────────────────────
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return BadRequest();
            var book = await _bookRepository.GetByIdAsync(id.Value);
            if (book == null) return NotFound();
            return View(book);
        }

        // ── POST: /Books/Edit/1 ───────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("BookId,Title,Author,ISBN,Genre,PublishedDate,Price,IsAvailable,Description")]
            Book book)
        {
            if (id != book.BookId) return BadRequest();

            if (ModelState.IsValid)
            {
                await _bookRepository.UpdateAsync(book);
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // ── GET: /Books/Delete/1 ──────────────────────────
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();
            var book = await _bookRepository.GetByIdAsync(id.Value);
            if (book == null) return NotFound();
            return View(book);
        }

        // ── POST: /Books/Delete/1 ─────────────────────────
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _bookRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
