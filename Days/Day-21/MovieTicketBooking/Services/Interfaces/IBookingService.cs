using MovieTicketBooking.DTOs.Bookings;

namespace MovieTicketBooking.Services.Interfaces
{
    public interface IBookingService
    {
        Task<List<BookingResponseDto>> GetAllAsync(
            BookingQueryDto query,
            string userId,
            bool isAdmin);

        Task<BookingResponseDto?> GetByIdAsync(
            int id,
            string userId,
            bool isAdmin);

        Task<BookingResponseDto> CreateAsync(
            CreateBookingDto dto,
            string userId);

        Task<bool> CancelAsync(
            int id,
            string userId,
            bool isAdmin);
    }
}
