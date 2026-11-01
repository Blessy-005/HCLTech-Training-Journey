namespace MovieTicketBooking.DTOs.Screens
{
    public class ScreenResponseDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int TheatreId { get; set; }

        public string TheatreName { get; set; } = string.Empty;

        public int TotalSeats { get; set; }

        public bool IsActive { get; set; }
    }
}