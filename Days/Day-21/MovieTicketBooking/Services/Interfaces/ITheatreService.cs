using MovieTicketBooking.DTOs.Theatres;

namespace MovieTicketBooking.Services.Interfaces
{
    public interface ITheatreService
    {
        Task<IEnumerable<TheatreResponseDto>> GetAllAsync(
            TheatreQueryDto query);

        Task<TheatreResponseDto?> GetByIdAsync(int id);

        Task<TheatreResponseDto> CreateAsync(
            CreateTheatreDto dto);

        Task<bool> UpdateAsync(
            int id,
            UpdateTheatreDto dto);

        Task<bool> DeleteAsync(int id);
    }
}