
namespace Models
{
    public class RentalHistoryDto
    {
        public int RentalId { get; set; }
        public DateTime RentalDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool IsOverdue { get; set; }
        public string BookName { get; set; }
        public string UserName { get; set; }
    }

}
