using MovieTicketBooking.DTOs.Seats;

namespace MovieTicketBooking.Services.Interfaces
{
    public interface ISeatService
    {
        Task<IEnumerable<SeatResponseDto>> GetAllAsync(
            SeatQueryDto query);

        Task<SeatResponseDto?> GetByIdAsync(int id);

        Task<SeatResponseDto> CreateAsync(
            CreateSeatDto dto);

        Task<IEnumerable<SeatResponseDto>> CreateBulkAsync(
            IEnumerable<CreateSeatDto> seats);

        Task<bool> UpdateAsync(
            int id,
            UpdateSeatDto dto);

        Task<bool> DeleteAsync(int id);
    }
}