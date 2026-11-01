using MovieTicketBooking.DTOs.ShowSeats;

namespace MovieTicketBooking.Services.Interfaces
{
    public interface IShowSeatService
    {
        Task<List<ShowSeatResponseDto>> GetByShowIdAsync(
            ShowSeatQueryDto query);
    }
}