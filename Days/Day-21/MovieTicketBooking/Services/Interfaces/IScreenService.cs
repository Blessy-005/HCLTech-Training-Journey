using MovieTicketBooking.DTOs.Screens;

namespace MovieTicketBooking.Services.Interfaces
{
    public interface IScreenService
    {
        Task<IEnumerable<ScreenResponseDto>> GetAllAsync(
            ScreenQueryDto query);

        Task<ScreenResponseDto?> GetByIdAsync(int id);

        Task<ScreenResponseDto> CreateAsync(
            CreateScreenDto dto);

        Task<bool> UpdateAsync(
            int id,
            UpdateScreenDto dto);

        Task<bool> DeleteAsync(int id);
    }
}