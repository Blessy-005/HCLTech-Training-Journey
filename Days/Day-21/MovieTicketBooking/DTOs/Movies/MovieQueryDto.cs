using System.ComponentModel.DataAnnotations;

namespace MovieTicketBooking.DTOs.Movies
{
    public class MovieQueryDto
    {
        public string? Search { get; set; }

        public string? Language { get; set; }

        public string? Genre { get; set; }

        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100)]
        public int PageSize { get; set; } = 10;
    }
}