using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using Data;
using Microsoft.EntityFrameworkCore;

namespace BackgroundServices.Services
{
    public class OverdueRentalService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public OverdueRentalService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Run the job every day at midnight
                    var nextRun = DateTime.UtcNow.Date.AddDays(1);
                    var delay = nextRun - DateTime.UtcNow;

                    if (delay > TimeSpan.Zero)
                        await Task.Delay(delay, stoppingToken);

                    await MarkOverdueRentalsAsync();
                }
                catch (Exception ex)
                {
                    // Log exception (use a proper logging mechanism here)
                    Console.WriteLine($"Error in OverdueRentalService: {ex.Message}");
                }
            }
        }

        private async Task MarkOverdueRentalsAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var overdueRentals = await dbContext.Rentals
                    .Where(r => r.ReturnDate == null && r.RentalDate.AddDays(14) < DateTime.UtcNow)
                    .ToListAsync();

                foreach (var rental in overdueRentals)
                {
                    rental.IsOverdue = true;
                }

                await dbContext.SaveChangesAsync();
                Console.WriteLine($"{overdueRentals.Count} rentals marked as overdue.");
            }
        }
    }
}
