using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories
{
    public class ShowSeatRepository : IShowSeatRepository
    {
        private readonly ApplicationDbContext _context;

        public ShowSeatRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ShowSeat>> GetByShowIdAsync(
            int showId,
            string? status)
        {
            var query = _context.ShowSeats
                .Include(ss => ss.Seat)
                    .ThenInclude(s => s.Screen)
                .Include(ss => ss.Show)
                .Where(ss => ss.ShowId == showId)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(ss => ss.Status == status);
            }

            return await query
                .OrderBy(ss => ss.Seat.SeatNumber)
                .ToListAsync();
        }

        public async Task<ShowSeat?> GetByIdAsync(int id)
        {
            return await _context.ShowSeats
                .Include(ss => ss.Seat)
                    .ThenInclude(s => s.Screen)
                .Include(ss => ss.Show)
                .FirstOrDefaultAsync(ss => ss.Id == id);
        }

        public async Task AddRangeAsync(IEnumerable<ShowSeat> showSeats)
        {
            await _context.ShowSeats.AddRangeAsync(showSeats);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ShowSeat showSeat)
        {
            _context.ShowSeats.Update(showSeat);
            await _context.SaveChangesAsync();
        }
    }
}