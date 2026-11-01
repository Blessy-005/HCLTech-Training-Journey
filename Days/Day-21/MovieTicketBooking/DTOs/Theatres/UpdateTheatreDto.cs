using System.ComponentModel.DataAnnotations;

namespace MovieTicketBooking.DTOs.Theatres
{
    public class UpdateTheatreDto
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(300)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string City { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}