using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces
{
    public interface IScreenRepository
    {
        Task<IEnumerable<Screen>> GetAllAsync(
            string? search,
            int? theatreId,
            int pageNumber,
            int pageSize);

        Task<Screen?> GetByIdAsync(int id);

        Task<Screen> AddAsync(Screen screen);

        Task UpdateAsync(Screen screen);

        Task DeleteAsync(Screen screen);
    }
}