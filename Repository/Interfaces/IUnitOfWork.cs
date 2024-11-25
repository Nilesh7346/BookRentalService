using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IBookRepository Books { get; }
        IUserRepository Users { get; }
        IRentalRepository Rentals { get; }
        Task CompleteAsync();
    }

}
