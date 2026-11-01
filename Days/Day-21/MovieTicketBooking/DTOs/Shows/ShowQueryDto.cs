using System.ComponentModel.DataAnnotations;

namespace MovieTicketBooking.DTOs.Shows
{
    public class ShowQueryDto
    {
        public int? MovieId { get; set; }

        public int? ScreenId { get; set; }

        public DateTime? ShowDate { get; set; }

        public bool? IsActive { get; set; }

        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100)]
        public int PageSize { get; set; } = 10;
    }
}