using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces
{
    public interface IMovieRepository
    {
        Task<IEnumerable<Movie>> GetAllAsync(
            string? search,
            string? language,
            string? genre,
            int pageNumber,
            int pageSize);

        Task<Movie?> GetByIdAsync(int id);

        Task<Movie> AddAsync(Movie movie);

        Task UpdateAsync(Movie movie);

        Task DeleteAsync(Movie movie);
    }
}