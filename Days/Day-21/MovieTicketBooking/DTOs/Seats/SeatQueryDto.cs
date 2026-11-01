using System.ComponentModel.DataAnnotations;

namespace MovieTicketBooking.DTOs.Seats
{
    public class SeatQueryDto
    {
        public string? Search { get; set; }

        public int? ScreenId { get; set; }

        public string? SeatType { get; set; }

        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100)]
        public int PageSize { get; set; } = 10;
    }
}