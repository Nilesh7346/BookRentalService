using Models;

namespace Data.Interfaces
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllAsync();
        Task<Book?> GetByIdAsync(int id);
        Task AddAsync(Book book);
        void Update(Book book);
        void Delete(Book book);
        Task<IEnumerable<BookDto>> SearchByTitleOrGenreAsync(string? title, string? genre);
    }
}
