using Models;
namespace Services
{
    public interface IBookRentalService
    {
        Task<IEnumerable<BookDto>> SearchBooksAsync(string? title, string? genre);
        Task<bool> RentBookAsync(int bookId, int userId);

        Task<BookStatisticsDto?> GetLeastPopularBookAsync();
        Task<BookStatisticsDto?> GetMostPopularBookAsync();
        Task<BookStatisticsDto?> GetMostOverdueBookAsync();

        Task<IEnumerable<RentalHistoryDto>> GetRentalHistoryByUserIdAsync(int userId);

        Task<IEnumerable<RentalHistoryDto>> GetRentalHistoryByBookIdAsync(int bookId);
       
        Task<bool> ReturnBookAsync(int bookId, int userId);
    }

}
