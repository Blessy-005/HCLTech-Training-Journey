namespace MovieTicketBooking.DTOs.Seats
{
    public class SeatResponseDto
    {
        public int Id { get; set; }

        public string SeatNumber { get; set; } = string.Empty;

        public string SeatType { get; set; } = string.Empty;

        public int ScreenId { get; set; }

        public string ScreenName { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }
}