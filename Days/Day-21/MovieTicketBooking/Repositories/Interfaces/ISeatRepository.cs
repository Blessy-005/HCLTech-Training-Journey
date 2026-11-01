using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces
{
    public interface ISeatRepository
    {
        Task<IEnumerable<Seat>> GetAllAsync(
            string? search,
            int? screenId,
            string? seatType,
            int pageNumber,
            int pageSize);

        Task<Seat?> GetByIdAsync(int id);

        Task<Seat> AddAsync(Seat seat);

        Task AddRangeAsync(IEnumerable<Seat> seats);

        Task UpdateAsync(Seat seat);

        Task DeleteAsync(Seat seat);

        Task<bool> SeatNumberExistsAsync(
            int screenId,
            string seatNumber,
            int? excludeSeatId = null);
    }
}