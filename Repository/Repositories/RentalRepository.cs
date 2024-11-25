using Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Transactions;
namespace Data.Repositories
{
    public class RentalRepository : IRentalRepository
    {
        private readonly AppDbContext _context;

        public RentalRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Rental>> GetAllAsync()
        {
            try
            {
                return await _context.Rentals.Include(r => r.Book).Include(r => r.User).ToListAsync();
            }
            catch (Exception ex)
            {
                // Log the exception and rethrow it if needed
                throw new Exception("An error occurred while processing the rental", ex);
            }
        }

        public async Task<Rental?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Rentals.Include(r => r.Book).Include(r => r.User).FirstOrDefaultAsync(r => r.Id == id);
            }
            catch (Exception ex)
            {
                // Log the exception and rethrow it if needed
                throw new Exception("An error occurred while processing the rental", ex);
            }
        }

        public async Task AddAsync(Rental rental)
        {
            try
            {
                await _context.Rentals.AddAsync(rental);
            }
            catch (Exception ex)
            {
                // Log the exception and rethrow it if needed
                throw new Exception("An error occurred while processing the rental", ex);
            }
        }


        public async Task<IEnumerable<Rental>> GetOverdueRentalsAsync()
        {
            try
            {
                return await _context.Rentals
                .Where(r => r.ReturnDate == null && r.RentalDate.AddDays(14) < DateTime.UtcNow)
                .Include(r => r.Book)
                .Include(r => r.User)
                .ToListAsync();
            }
            catch (Exception ex)
            {
                // Log the exception and rethrow it if needed
                throw new Exception("An error occurred while processing the OverdueRentals", ex);
            }
        }

        // Most Overdue Book
        public async Task<BookStatisticsDto?> GetMostOverdueBookAsync()
        {
            try
            {
                var overdueBooks = await _context.Rentals
                .Where(r => r.IsOverdue == true)
                .GroupBy(r => r.BookId)
                .Select(g => new
                {
                    BookId = g.Key,
                    RentalCount = g.Count()
                })
                .Join(_context.Books, r => r.BookId, b => b.Id, (r, b) => new BookStatisticsDto
                {
                    BookId = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    RentalCount = r.RentalCount
                })
                .OrderByDescending(b => b.RentalCount)
                .FirstOrDefaultAsync();

                if (overdueBooks == null)
                {
                    // Return null or handle the case where no overdue books exist
                    return null;
                }
                return overdueBooks;
            }
            catch (Exception ex)
            {
                // Log the exception and rethrow it if needed
                throw new Exception("An error occurred while processing the MostOverdueBook", ex);
            }

        }


        // Most Popular Book (based on rental count)
        public async Task<BookStatisticsDto?> GetMostPopularBookAsync()
        {
            try
            {
                var mostPopularBook = await _context.Rentals
                 .GroupBy(r => r.BookId)
                 .Select(g => new
                 {
                     BookId = g.Key,
                     RentalCount = g.Count()
                 })
                 .Join(_context.Books, r => r.BookId, b => b.Id, (r, b) => new BookStatisticsDto
                 {
                     BookId = b.Id,
                     Title = b.Title,
                     Author = b.Author,
                     RentalCount = r.RentalCount
                 })
                 .OrderByDescending(b => b.RentalCount)
                 .FirstOrDefaultAsync();

                if (mostPopularBook == null)
                {
                    // Return null or handle the case where no overdue books exist
                    return null;
                }
                return mostPopularBook;
            }
            catch (Exception ex)
            {
                // Log the exception and rethrow it if needed
                throw new Exception("An error occurred while processing the MostPopularBook", ex);
            }

        }

        // Least Popular Book (based on rental count)
        public async Task<BookStatisticsDto?> GetLeastPopularBookAsync()
        {
            try
            {
                var leastPopularBook = await _context.Rentals
                .GroupBy(r => r.BookId)
                .Select(g => new
                {
                    BookId = g.Key,
                    RentalCount = g.Count()
                })
                .Join(_context.Books, r => r.BookId, b => b.Id, (r, b) => new BookStatisticsDto
                {
                    BookId = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    RentalCount = r.RentalCount
                })
                .OrderBy(b => b.RentalCount)
                .FirstOrDefaultAsync();

                if (leastPopularBook == null)
                {
                    // Return null or handle the case where no overdue books exist
                    return null;
                }
                return leastPopularBook;
            }
            catch (Exception ex)
            {
                // Log the exception and rethrow it if needed
                throw new Exception("An error occurred while processing the GetLeastPopularBook", ex);
            }

        }

        public async Task<bool> RentBookAsync(int bookId, int userId)
        {
            try
            {
                // Use async support for TransactionScope
                var transactionOptions = new TransactionOptions
                {
                    IsolationLevel = IsolationLevel.Serializable // Ensures row locking at the database level
                };

                // Creating the transaction scope with async flow enabled
                using (TransactionScope transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var book = _context.Books.FirstOrDefault(item => item.Id == bookId);
                    if (book == null || book.AvailableCopies <= 0)
                        throw new Exception("Book is not available.");

                    var rental = new Rental
                    {
                        BookId = bookId,
                        UserId = userId,
                        RentalDate = DateTime.UtcNow
                    };

                    // Lock to prevent race condition in the data layer
                    lock (book)
                    {
                        book.AvailableCopies--;
                    }

                    await _context.AddAsync(rental);

                    // Complete the transaction only after all operations are successful
                    transactionScope.Complete();
                }
            }
            catch (Exception ex)
            {
                // Log the exception and rethrow it if needed
                throw new Exception(ex.Message);
            }
            return true;
        }

        public async Task<bool> ReturnBookAsync(int bookId, int userId)
        {
            try
            {
                // Use async support for TransactionScope
                var transactionOptions = new TransactionOptions
                {
                    IsolationLevel = IsolationLevel.Serializable // Ensures row locking at the database level
                };

                // Creating the transaction scope with async flow enabled
                using (TransactionScope transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var rentalHistory = _context.Rentals.Where(item => item.BookId == bookId && item.UserId == userId && item.ReturnDate == null).Select(record => new { record, record.Book }).FirstOrDefault();
                    if (rentalHistory == null)
                        throw new Exception("No book rent record available.");

                    // Lock to prevent race condition in the data layer
                    lock (rentalHistory)
                    {
                        rentalHistory.Book.AvailableCopies++;
                        rentalHistory.record.ReturnDate = DateTime.UtcNow;
                    }

                    await _context.SaveChangesAsync();

                    // Complete the transaction only after all operations are successful
                    transactionScope.Complete();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while processing the ReturnBookAsync :" + ex.Message);
            }
            return true;
        }

        public async Task<IEnumerable<RentalHistoryDto>> GetRentalHistoryByUserIdAsync(int userId)
        {
            try
            {
                return await _context.Rentals
                    .Where(r => r.UserId == userId)
                    .Include(r => r.Book)
                    .Include(r => r.User)
                    .Select(r => new RentalHistoryDto
                    {
                        RentalId = r.Id,
                        RentalDate = r.RentalDate,
                        ReturnDate = r.ReturnDate,
                        IsOverdue = r.IsOverdue,
                        BookName = r.Book.Title,
                        UserName = r.User.Name
                    })
                    .OrderByDescending(r => r.RentalDate) // Sort by rental date
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                // Log the exception and rethrow it if needed
                throw new Exception("An error occurred while processing the GetRentalHistoryByUserId", ex);
            }
        }

        public async Task<IEnumerable<RentalHistoryDto>> GetRentalHistoryByBookIdAsync(int bookId)
        {
            try
            {
                return await _context.Rentals
                    .Where(r => r.BookId == bookId)
                    .Include(r => r.Book)
                    .Include(r => r.User)
                    .Select(r => new RentalHistoryDto
                    {
                        RentalId = r.Id,
                        RentalDate = r.RentalDate,
                        ReturnDate = r.ReturnDate,
                        IsOverdue = r.IsOverdue,
                        BookName = r.Book.Title,
                        UserName = r.User.Name
                    })
                    .OrderByDescending(r => r.RentalDate) // Sort by rental date
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                // Log the exception and rethrow it if needed
                throw new Exception("An error occurred while processing the RentalHistoryByBookId", ex);
            }
        }
    }
}
