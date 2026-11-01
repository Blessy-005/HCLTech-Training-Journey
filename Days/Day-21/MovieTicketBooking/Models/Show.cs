using System;

namespace MovieTicketBooking.Models
{
    public class Show
    {
        public int Id { get; set; }

        public int MovieId { get; set; }

        public int ScreenId { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public decimal TicketPrice { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public Movie? Movie { get; set; }

        public Screen? Screen { get; set; }
    }
}