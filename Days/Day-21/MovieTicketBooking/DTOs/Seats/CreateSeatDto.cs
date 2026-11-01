using System.ComponentModel.DataAnnotations;

namespace MovieTicketBooking.DTOs.Seats
{
    public class CreateSeatDto
    {
        [Required]
        [MaxLength(10)]
        public string SeatNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(30)]
        public string SeatType { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int ScreenId { get; set; }
    }
}