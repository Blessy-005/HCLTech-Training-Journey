using System.ComponentModel.DataAnnotations;

namespace MovieTicketBooking.DTOs.Movies
{
    public class UpdateMovieDto
    {
        [Required]
        [MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Language { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Genre { get; set; } = string.Empty;

        [Range(1, 500)]
        public int DurationMinutes { get; set; }

        [MaxLength(500)]
        public string PosterUrl { get; set; } = string.Empty;

        public DateTime ReleaseDate { get; set; }

        public bool IsActive { get; set; } = true;
    }
}