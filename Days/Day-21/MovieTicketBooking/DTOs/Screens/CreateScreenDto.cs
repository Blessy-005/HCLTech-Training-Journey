using System.ComponentModel.DataAnnotations;

namespace MovieTicketBooking.DTOs.Screens
{
    public class CreateScreenDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int TheatreId { get; set; }

        [Range(1, 1000)]
        public int TotalSeats { get; set; }
    }
}