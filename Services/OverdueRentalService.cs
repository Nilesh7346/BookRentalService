using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Data;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using Microsoft.Extensions.Options;
using Services.Models;
using MailKit.Net.Smtp;

namespace Services
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
                    .Where(r => r.ReturnDate == null && !r.IsOverdue && r.RentalDate.AddDays(14) < DateTime.UtcNow)
                    .ToListAsync();


                foreach (var rental in overdueRentals)
                {
                    rental.IsOverdue = true;
                    var userDetails = dbContext.Rentals.Where(r => r.Id == rental.Id)
                           .Select(r => new
                           {
                               r.User.Name,
                               r.User.Email,
                               r.Book.Title
                           })
                           .FirstOrDefault();
                    if (userDetails != null)
                        await SendOverdueEmailNotification(userDetails.Name, userDetails.Email, userDetails.Title);
                }

                await dbContext.SaveChangesAsync();
                Console.WriteLine($"{overdueRentals.Count} rentals marked as overdue.");
            }
        }

        private async Task SendOverdueEmailNotification(string userName, string userEmail, string bookTitle)
        {
            var emailSettings = _serviceProvider.GetRequiredService<IOptions<EmailSettings>>().Value;

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(emailSettings.SenderName, emailSettings.SenderEmail));
            emailMessage.To.Add(new MailboxAddress(userName, userEmail));
            emailMessage.Subject = "Overdue Rental Notification";
            emailMessage.Body = new TextPart("plain")
            {
                Text = $"Dear {userName},\n\nThe rental period for the book \"{bookTitle}\" has exceeded 14 days. Please return the book as soon as possible to avoid further actions.\n\nThank you,\nBook Rental Service"
            };

            using (var smtpClient = new SmtpClient())
            {
                await smtpClient.ConnectAsync(emailSettings.SmtpServer, emailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
                await smtpClient.AuthenticateAsync(emailSettings.SenderEmail, emailSettings.SenderPassword);
                await smtpClient.SendAsync(emailMessage);
                await smtpClient.DisconnectAsync(true);
            }

            Console.WriteLine($"Email sent to {userEmail} for overdue book \"{bookTitle}\".");
        }
    }
}
