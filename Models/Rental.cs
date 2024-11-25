using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Rental
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public DateTime RentalDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool IsOverdue { get; set; } = false;

        [Timestamp]
        public byte[] RowVersion { get; set; } // Concurrency token
    }

}
