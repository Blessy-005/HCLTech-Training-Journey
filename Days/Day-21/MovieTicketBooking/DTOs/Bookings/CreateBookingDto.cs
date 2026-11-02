
using System.ComponentModel.DataAnnotations;

namespace MovieTicketBooking.DTOs.Bookings
{
    public class CreateBookingDto
    {
        [Required]
        [MinLength(1)]
        public List<int> ShowSeatIds { get; set; } = new();
    }
}
