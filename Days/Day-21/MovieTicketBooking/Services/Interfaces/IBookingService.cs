using MovieTicketBooking.DTOs.Bookings;

namespace MovieTicketBooking.Services.Interfaces
{
    public interface IBookingService
    {
        Task<List<BookingResponseDto>> GetAllAsync(
            BookingQueryDto query);

        Task<BookingResponseDto?> GetByIdAsync(int id);

        Task<BookingResponseDto> CreateAsync(CreateBookingDto dto);

        Task<bool> CancelAsync(int id);
    }
}