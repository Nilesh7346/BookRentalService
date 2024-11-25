using Microsoft.EntityFrameworkCore;
using Models;
using Data.Interfaces;

namespace Data.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly AppDbContext _context;

        public BookRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Book>> GetAllAsync()
        {
            return await _context.Books.ToListAsync();
        }

        public async Task<Book?> GetByIdAsync(int id)
        {
            return await _context.Books.FindAsync(id);
        }

        public async Task AddAsync(Book book)
        {
            await _context.Books.AddAsync(book);
        }

        public void Update(Book book)
        {
            _context.Books.Update(book);
        }

        public void Delete(Book book)
        {
            _context.Books.Remove(book);
        }

        public async Task<IEnumerable<BookDto>> SearchByTitleOrGenreAsync(string? title, string? genre)
        {
            return await _context.Books
                .Where(b => (string.IsNullOrEmpty(title) || b.Title.Contains(title)) &&
                            (string.IsNullOrEmpty(genre) || b.Genre.Contains(genre)))
                 .Select(book => new BookDto
                 {
                     Id = book.Id,
                     Title = book.Title,
                     Author = book.Author,
                     ISBN = book.ISBN,
                     Genre = book.Genre,
                     AvailableCopies = book.AvailableCopies,
                     TotalCopies = book.TotalCopies
                 }).ToListAsync();
        }
    }

}
