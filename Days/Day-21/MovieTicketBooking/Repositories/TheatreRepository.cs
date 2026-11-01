using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories
{
    public class TheatreRepository : ITheatreRepository
    {
        private readonly ApplicationDbContext _context;

        public TheatreRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Theatre>> GetAllAsync(
            string? search,
            string? city,
            int pageNumber,
            int pageSize)
        {
            var query = _context.Theatres.AsQueryable();

            // Show only active theatres
            query = query.Where(t => t.IsActive);

            // Search by theatre name
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(t =>
                    t.Name.Contains(search));
            }

            // Filter by city
            if (!string.IsNullOrWhiteSpace(city))
            {
                query = query.Where(t =>
                    t.City == city);
            }

            return await query
                .OrderBy(t => t.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Theatre?> GetByIdAsync(int id)
        {
            return await _context.Theatres
                .FirstOrDefaultAsync(t =>
                    t.Id == id && t.IsActive);
        }

        public async Task<Theatre> AddAsync(Theatre theatre)
        {
            await _context.Theatres.AddAsync(theatre);
            await _context.SaveChangesAsync();

            return theatre;
        }

        public async Task UpdateAsync(Theatre theatre)
        {
            _context.Theatres.Update(theatre);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Theatre theatre)
        {
            _context.Theatres.Remove(theatre);
            await _context.SaveChangesAsync();
        }
    }
}