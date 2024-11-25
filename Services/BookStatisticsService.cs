using Data;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Services
{
    public class BookStatisticsService
    {
        private readonly AppDbContext _context;

        public BookStatisticsService(AppDbContext context)
        {
            _context = context;
        }

     
    }

}
