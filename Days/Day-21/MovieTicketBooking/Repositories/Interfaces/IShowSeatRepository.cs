using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces
{
    public interface IShowSeatRepository
    {
        Task<List<ShowSeat>> GetByShowIdAsync(
            int showId,
            string? status);

        Task<ShowSeat?> GetByIdAsync(int id);

        Task AddRangeAsync(IEnumerable<ShowSeat> showSeats);

        Task UpdateAsync(ShowSeat showSeat);
        Task<List<ShowSeat>> GetByIdsAsync(List<int> showSeatIds);
    }
}