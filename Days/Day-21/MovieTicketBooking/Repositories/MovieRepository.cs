using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private readonly ApplicationDbContext _context;

        public MovieRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Movie> AddAsync(Movie movie)
        {
            await _context.Movies.AddAsync(movie);

            await _context.SaveChangesAsync();

            return movie;
        }

        public async Task DeleteAsync(Movie movie)
        {
            _context.Movies.Remove(movie);

            await _context.SaveChangesAsync();
        }


        public async Task<IEnumerable<Movie>> GetAllAsync(
            string? search,
            string? language,
            string? genre,
            int pageNumber,
            int pageSize)
        {
            var query = _context.Movies
                .Where(m => m.IsActive)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(m =>
                    m.Title.Contains(search.Trim()));
            }

            if (!string.IsNullOrWhiteSpace(language))
            {
                query = query.Where(m =>
                    m.Language == language.Trim());
            }

            if (!string.IsNullOrWhiteSpace(genre))
            {
                query = query.Where(m =>
                    m.Genre == genre.Trim());
            }

            return await query
                .OrderBy(m => m.Title)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Movie?> GetByIdAsync(int id)
        {
            return await _context.Movies
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task UpdateAsync(Movie movie)
        {
            _context.Movies.Update(movie);

            await _context.SaveChangesAsync();
        }
    }
}