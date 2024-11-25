
using Microsoft.EntityFrameworkCore;
using Models;

namespace Data
{

    public class AppDbContext : DbContext
    {
        // Constructor to configure the context
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSet properties for each model
        public DbSet<Book> Books { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Rental> Rentals { get; set; }

        public DbSet<ActivityLog> ActivityLogs { get; set; }

        // Configuring the model relationships and database schema
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Book entity
            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasKey(b => b.Id);
                entity.Property(b => b.Title).IsRequired().HasMaxLength(200);
                entity.Property(b => b.Author).IsRequired().HasMaxLength(100);
                entity.Property(b => b.ISBN).IsRequired().HasMaxLength(13);
                entity.Property(b => b.Genre).IsRequired().HasMaxLength(50);
                entity.Property(b => b.AvailableCopies).IsRequired();
                entity.Property(b => b.TotalCopies).IsRequired();
                entity.Property(b => b.RowVersion).IsRowVersion();
            });

            // User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Name).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(150);
            });

            // Rental entity
            modelBuilder.Entity<Rental>(entity =>
            {
                entity.HasKey(r => r.Id);

                entity.Property(r => r.RentalDate).IsRequired();
                entity.Property(r => r.IsOverdue);

                // Relationships
                entity.HasOne(r => r.Book)
                      .WithMany()
                      .HasForeignKey(r => r.BookId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(r => r.User)
                      .WithMany()
                      .HasForeignKey(r => r.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.Property(b => b.RowVersion).IsRowVersion();
            });

            modelBuilder.Entity<ActivityLog>(entity =>
            {
                entity.HasKey(e => e.Id); // Primary key

                entity.Property(e => e.LogType)
                    .IsRequired()
                    .HasMaxLength(50); // Limit LogType to 50 characters

                entity.Property(e => e.Message)
                    .IsRequired(); // Message is required

                entity.Property(e => e.LogTime)
                    .IsRequired()
                    .HasDefaultValueSql("GETDATE()"); // Default value for LogTime

                entity.Property(e => e.Endpoint)
                    .HasMaxLength(200); // Limit Endpoint to 200 characters

                entity.Property(e => e.UserId)
                    .IsRequired(false); // UserId is optional

                entity.Property(e => e.DurationMs)
                    .IsRequired(false); // DurationMs is optional
            });
            
        }
    }

}
