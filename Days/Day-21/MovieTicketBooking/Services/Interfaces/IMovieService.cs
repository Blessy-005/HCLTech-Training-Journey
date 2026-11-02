using MovieTicketBooking.DTOs.Common;
using MovieTicketBooking.DTOs.Movies;

namespace MovieTicketBooking.Services.Interfaces
{
    public interface IMovieService
    {
        Task<PaginationResponseDto<MovieResponseDto>> GetAllAsync(
    MovieQueryDto query);

        Task<MovieResponseDto?> GetByIdAsync(int id);

        Task<MovieResponseDto> CreateAsync(CreateMovieDto dto);

        Task<bool> UpdateAsync(int id, UpdateMovieDto dto);

        Task<bool> DeleteAsync(int id);
    }
}