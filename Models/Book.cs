using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Book
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Author { get; set; }
        public required string ISBN { get; set; }
        public required string Genre { get; set; }
        public int AvailableCopies { get; set; }
        public int TotalCopies { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; } // Concurrency token
    }

}
