using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Interfaces
{
    public interface IRentalRepository
    {
        Task<IEnumerable<Rental>> GetAllAsync();
        Task<Rental?> GetByIdAsync(int id);
        Task AddAsync(Rental rental);

        Task<bool> ReturnBookAsync(int bookId, int userId);

        Task<IEnumerable<Rental>> GetOverdueRentalsAsync();
        Task<BookStatisticsDto?> GetLeastPopularBookAsync();
        Task<BookStatisticsDto?> GetMostPopularBookAsync();
        Task<BookStatisticsDto?> GetMostOverdueBookAsync();
        Task<bool> RentBookAsync(int bookId, int userId);
        Task<IEnumerable<RentalHistoryDto>> GetRentalHistoryByUserIdAsync(int userId);

        Task<IEnumerable<RentalHistoryDto>> GetRentalHistoryByBookIdAsync(int userId);
    }


}
