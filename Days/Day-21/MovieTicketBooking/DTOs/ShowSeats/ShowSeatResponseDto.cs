namespace MovieTicketBooking.DTOs.ShowSeats
{
    public class ShowSeatResponseDto
    {
        public int Id { get; set; }

        public int ShowId { get; set; }

        public int SeatId { get; set; }

        public string SeatNumber { get; set; } = string.Empty;

        public string SeatType { get; set; } = string.Empty;

        public string ScreenName { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public decimal TicketPrice { get; set; }
    }
}