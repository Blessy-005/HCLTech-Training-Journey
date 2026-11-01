namespace MovieTicketBooking.Models
{
    public class Seat
    {
        public int Id { get; set; }

        public int ScreenId { get; set; }

        public string SeatNumber { get; set; } = string.Empty;

        public string SeatType { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        // Navigation property
        public Screen? Screen { get; set; }
    }
}