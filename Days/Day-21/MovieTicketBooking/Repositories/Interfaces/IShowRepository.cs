using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces
{
    public interface IShowRepository
    {
        Task<List<Show>> GetAllAsync(
            int? movieId,
            int? screenId,
            DateTime? showDate,
            bool? isActive,
            int pageNumber,
            int pageSize);

        Task<Show?> GetByIdAsync(int id);

        Task<Show> AddAsync(Show show);

        Task<Show> UpdateAsync(Show show);

        Task<bool> DeleteAsync(Show show);

        Task<bool> HasOverlappingShowAsync(
            int screenId,
            DateTime startTime,
            DateTime endTime,
            int? excludeShowId = null);
    }
}