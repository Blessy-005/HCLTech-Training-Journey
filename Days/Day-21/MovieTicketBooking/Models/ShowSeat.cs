namespace MovieTicketBooking.Models
{
    public class ShowSeat
    {
        public int Id { get; set; }

        public int ShowId { get; set; }

        public int SeatId { get; set; }

        public string Status { get; set; } = "Available";

        // Navigation properties
        public Show? Show { get; set; }

        public Seat? Seat { get; set; }
    }
}