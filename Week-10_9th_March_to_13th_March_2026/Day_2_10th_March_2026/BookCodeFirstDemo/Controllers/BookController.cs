using BookCodeFirstDemo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace BookCodeFirstDemo.Controllers
{
    public class BookController : Controller
    {
        private readonly BookDBContext _context;

        public BookController(BookDBContext context)
        {
            _context = context;
        }

        // GET: Book
        public IActionResult Index()
        {
            var books = _context.books.ToList();
            return View(books);
        }

        // GET: Book/Details/5
        public IActionResult Details(int id)
        {
            var book = _context.books.FirstOrDefault(b => b.BookId == id);

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Book/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Book/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BookModel book)
        {
            if (ModelState.IsValid)
            {
                _context.books.Add(book);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(book);
        }

        // GET: Book/Edit/5
        public IActionResult Edit(int id)
        {
            var book = _context.books.Find(id);

            if (book == null)
                return NotFound();

            return View(book);
        }

        // POST: Book/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(BookModel book)
        {
            if (ModelState.IsValid)
            {
                _context.books.Update(book);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(book);
        }

        // GET: Book/Delete/5
        public IActionResult Delete(int id)
        {
            var book = _context.books.Find(id);

            if (book == null)
                return NotFound();

            return View(book);
        }

        // POST: Book/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var book = _context.books.Find(id);

            if (book != null)
            {
                _context.books.Remove(book);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}