using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces
{
    public interface ITheatreRepository
    {
        Task<IEnumerable<Theatre>> GetAllAsync(
            string? search,
            string? city,
            int pageNumber,
            int pageSize);

        Task<Theatre?> GetByIdAsync(int id);

        Task<Theatre> AddAsync(Theatre theatre);

        Task UpdateAsync(Theatre theatre);

        Task DeleteAsync(Theatre theatre);
    }
}