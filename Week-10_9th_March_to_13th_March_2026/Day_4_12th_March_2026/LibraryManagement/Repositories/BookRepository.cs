using Microsoft.EntityFrameworkCore;
using LibraryManagement.Data;
using LibraryManagement.Models;

namespace LibraryManagement.Repositories
{
    // =====================================================
    // PRACTICE QUESTION 1:
    // Concrete implementation of IBookRepository
    // =====================================================
    // BookRepository is the actual class that talks to the
    // database via EF Core. The controller never knows this
    // class exists — it only knows IBookRepository.
    //
    // This is the Repository Pattern:
    //   Controller → IBookRepository ← BookRepository → DB
    // =====================================================

    public class BookRepository : IBookRepository
    {
        // AppDbContext injected via constructor DI
        // Scoped lifetime: same DbContext instance
        // is shared within one HTTP request
        private readonly AppDbContext _context;

        public BookRepository(AppDbContext context)
            => _context = context;

        // ── READ ALL ─────────────────────────────────────
        public async Task<IEnumerable<Book>> GetAllAsync()
        {
            return await _context.Books
                .OrderBy(b => b.Title)
                .ToListAsync();
        }

        // ── READ BY ID ────────────────────────────────────
        // Returns null if not found (nullable Book?)
        // Used by /Books/Details/1
        public async Task<Book?> GetByIdAsync(int id)
        {
            return await _context.Books
                .FirstOrDefaultAsync(b => b.BookId == id);
        }

        // ── READ BY GENRE ─────────────────────────────────
        public async Task<IEnumerable<Book>> GetByGenreAsync(string genre)
        {
            return await _context.Books
                .Where(b => b.Genre.ToLower() == genre.ToLower())
                .OrderBy(b => b.Title)
                .ToListAsync();
        }

        // ── SEARCH ───────────────────────────────────────
        public async Task<IEnumerable<Book>> SearchAsync(string keyword)
        {
            return await _context.Books
                .Where(b => b.Title.Contains(keyword) ||
                            b.Author.Contains(keyword))
                .OrderBy(b => b.Title)
                .ToListAsync();
        }

        // ── CREATE ───────────────────────────────────────
        public async Task AddAsync(Book book)
        {
            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();
        }

        // ── UPDATE ───────────────────────────────────────
        public async Task UpdateAsync(Book book)
        {
            _context.Books.Update(book);
            await _context.SaveChangesAsync();
        }

        // ── DELETE ───────────────────────────────────────
        public async Task DeleteAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
        }

        // ── EXISTS ───────────────────────────────────────
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Books
                .AnyAsync(b => b.BookId == id);
        }
    }
}
