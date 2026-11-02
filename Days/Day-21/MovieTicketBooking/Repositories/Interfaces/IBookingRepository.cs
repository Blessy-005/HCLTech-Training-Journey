
using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces
{
    public interface IBookingRepository
    {
        Task<List<Booking>> GetAllAsync(
            string? customerEmail,
            string? status,
            DateTime? bookingDate,
            int pageNumber,
            int pageSize,
            string? userId);

        Task<Booking?> GetByIdAsync(int id);

        Task<Booking> AddAsync(Booking booking);

        Task UpdateAsync(Booking booking);
    }
}
