using Data.Interfaces;

namespace Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public IBookRepository Books { get; }
        public IUserRepository Users { get; }
        public IRentalRepository Rentals { get; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Books = new BookRepository(context);
            Users = new UserRepository(context);
            Rentals = new RentalRepository(context);
        }

        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }

}
