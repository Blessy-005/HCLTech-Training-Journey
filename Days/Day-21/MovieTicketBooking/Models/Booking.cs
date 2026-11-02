using System;
using System.Collections.Generic;

namespace MovieTicketBooking.Models
{
    public class Booking
    {
        public int Id { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public string CustomerEmail { get; set; } = string.Empty;

        // Link booking to the registered customer
        public string? UserId { get; set; }

        public ApplicationUser? User { get; set; }

        public DateTime BookingDate { get; set; } = DateTime.UtcNow;

        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = "Confirmed";

        // Navigation property
        public ICollection<BookingSeat> BookingSeats { get; set; }
            = new List<BookingSeat>();
    }
}