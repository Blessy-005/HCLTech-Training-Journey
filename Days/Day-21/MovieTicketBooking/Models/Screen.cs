namespace MovieTicketBooking.Models
{
    public class Screen
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int TheatreId { get; set; }

        public int TotalSeats { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation property
        public Theatre? Theatre { get; set; }
    }
}