using System.ComponentModel.DataAnnotations;

namespace MovieTicketBooking.DTOs.Theatres
{
    public class TheatreQueryDto
    {
        public string? Search { get; set; }

        public string? City { get; set; }

        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100)]
        public int PageSize { get; set; } = 10;
    }
}