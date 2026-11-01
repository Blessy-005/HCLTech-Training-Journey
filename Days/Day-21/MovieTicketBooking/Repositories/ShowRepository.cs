using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories
{
    public class ShowRepository : IShowRepository
    {
        private readonly ApplicationDbContext _context;

        public ShowRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Show>> GetAllAsync(
            int? movieId,
            int? screenId,
            DateTime? showDate,
            bool? isActive,
            int pageNumber,
            int pageSize)
        {
            var query = _context.Shows
                .Include(s => s.Movie)
                .Include(s => s.Screen)
                    .ThenInclude(sc => sc.Theatre)
                .AsQueryable();

            if (movieId.HasValue)
                query = query.Where(s => s.MovieId == movieId.Value);

            if (screenId.HasValue)
                query = query.Where(s => s.ScreenId == screenId.Value);

            if (showDate.HasValue)
            {
                var date = showDate.Value.Date;
                var nextDate = date.AddDays(1);

                query = query.Where(s =>
                    s.StartTime >= date && s.StartTime < nextDate);
            }

            if (isActive.HasValue)
                query = query.Where(s => s.IsActive == isActive.Value);

            return await query
                .OrderBy(s => s.StartTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Show?> GetByIdAsync(int id)
        {
            return await _context.Shows
                .Include(s => s.Movie)
                .Include(s => s.Screen!)
                    .ThenInclude(sc => sc.Theatre)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Show> AddAsync(Show show)
        {
            await _context.Shows.AddAsync(show);
            await _context.SaveChangesAsync();

            return show;
        }

        public async Task<Show> UpdateAsync(Show show)
        {
            _context.Shows.Update(show);
            await _context.SaveChangesAsync();

            return show;
        }

        public async Task<bool> DeleteAsync(Show show)
        {
            _context.Shows.Remove(show);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> HasOverlappingShowAsync(
            int screenId,
            DateTime startTime,
            DateTime endTime,
            int? excludeShowId = null)
        {
            var query = _context.Shows.Where(s =>
                s.ScreenId == screenId &&
                s.IsActive &&
                startTime < s.EndTime &&
                endTime > s.StartTime);

            if (excludeShowId.HasValue)
            {
                query = query.Where(s => s.Id != excludeShowId.Value);
            }

            return await query.AnyAsync();
        }
        public async Task<Show> AddWithSeatsAsync(
    Show show,
    List<Seat> seats)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                // Step 1: Save the Show record.
                await _context.Shows.AddAsync(show);
                await _context.SaveChangesAsync();

                // Step 2: Create availability records for every seat.
                var showSeats = seats.Select(seat => new ShowSeat
                {
                    ShowId = show.Id,
                    SeatId = seat.Id,
                    Status = "Available"
                }).ToList();

                // Step 3: Save all ShowSeat records.
                await _context.ShowSeats.AddRangeAsync(showSeats);
                await _context.SaveChangesAsync();

                // Step 4: Commit both operations together.
                await transaction.CommitAsync();

                return show;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}