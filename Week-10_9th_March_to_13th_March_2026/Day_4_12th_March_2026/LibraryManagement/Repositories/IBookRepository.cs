using LibraryManagement.Models;

namespace LibraryManagement.Repositories
{
    

    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllAsync();

        Task<Book?> GetByIdAsync(int id);

        Task<IEnumerable<Book>> GetByGenreAsync(string genre);

        Task<IEnumerable<Book>> SearchAsync(string keyword);

        Task AddAsync(Book book);

        Task UpdateAsync(Book book);

        Task DeleteAsync(int id);

        Task<bool> ExistsAsync(int id);
    }
}
