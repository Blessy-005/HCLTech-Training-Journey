using MovieTicketBooking.DTOs.Shows;

namespace MovieTicketBooking.Services.Interfaces
{
    public interface IShowService
    {
        Task<List<ShowResponseDto>> GetAllAsync(ShowQueryDto query);

        Task<ShowResponseDto?> GetByIdAsync(int id);

        Task<ShowResponseDto> CreateAsync(CreateShowDto dto);

        Task<bool> UpdateAsync(int id, UpdateShowDto dto);

        Task<bool> DeleteAsync(int id);
    }
}