using Models;
using Data.Interfaces;



namespace Services
{
    public class BookRentalService : IBookRentalService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookRentalService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<BookDto>> SearchBooksAsync(string? title, string? genre)
        {
            try
            {
                return await _unitOfWork.Books.SearchByTitleOrGenreAsync(title, genre);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> RentBookAsync(int bookId, int userId)
        {
            try
            {
                return await _unitOfWork.Rentals.RentBookAsync(bookId, userId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<BookStatisticsDto?> GetLeastPopularBookAsync()
        {
            try
            {
                return await _unitOfWork.Rentals.GetLeastPopularBookAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<BookStatisticsDto?> GetMostPopularBookAsync()
        {
            try
            {
                return await _unitOfWork.Rentals.GetMostPopularBookAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<BookStatisticsDto?> GetMostOverdueBookAsync()
        {
            try
            {
                return await _unitOfWork.Rentals.GetMostOverdueBookAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<RentalHistoryDto>> GetRentalHistoryByUserIdAsync(int userId)
        {
            try
            {
                return await _unitOfWork.Rentals.GetRentalHistoryByUserIdAsync(userId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<RentalHistoryDto>> GetRentalHistoryByBookIdAsync(int bookId)
        {
            try
            {
                return await _unitOfWork.Rentals.GetRentalHistoryByBookIdAsync(bookId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> ReturnBookAsync(int bookId, int userId)
        {
            try
            {
                return await _unitOfWork.Rentals.ReturnBookAsync(bookId, userId);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}


