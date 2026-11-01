namespace MovieTicketBooking.DTOs.Shows
{
    public class ShowResponseDto
    {
        public int Id { get; set; }

        public int MovieId { get; set; }
        public string MovieTitle { get; set; } = string.Empty;

        public int ScreenId { get; set; }
        public string ScreenName { get; set; } = string.Empty;

        public string TheatreName { get; set; } = string.Empty;

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public decimal TicketPrice { get; set; }

        public bool IsActive { get; set; }
    }
}