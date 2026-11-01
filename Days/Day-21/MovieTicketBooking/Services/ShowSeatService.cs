using MovieTicketBooking.DTOs.ShowSeats;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services
{
    public class ShowSeatService : IShowSeatService
    {
        private readonly IShowSeatRepository _showSeatRepository;

        public ShowSeatService(IShowSeatRepository showSeatRepository)
        {
            _showSeatRepository = showSeatRepository;
        }

        public async Task<List<ShowSeatResponseDto>> GetByShowIdAsync(
            ShowSeatQueryDto query)
        {
            var showSeats = await _showSeatRepository.GetByShowIdAsync(
                query.ShowId,
                query.Status);

            return showSeats.Select(ss => new ShowSeatResponseDto
            {
                Id = ss.Id,
                ShowId = ss.ShowId,
                SeatId = ss.SeatId,
                SeatNumber = ss.Seat?.SeatNumber ?? string.Empty,
                SeatType = ss.Seat?.SeatType ?? string.Empty,
                ScreenName = ss.Seat?.Screen?.Name ?? string.Empty,
                Status = ss.Status,
                TicketPrice = ss.Show?.TicketPrice ?? 0
            }).ToList();
        }
    }
}