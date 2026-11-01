using System.ComponentModel.DataAnnotations;

namespace MovieTicketBooking.DTOs.ShowSeats
{
    public class ShowSeatQueryDto
    {
        [Range(1, int.MaxValue)]
        public int ShowId { get; set; }

        public string? Status { get; set; }
    }
}