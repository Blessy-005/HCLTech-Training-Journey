using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories
{
    public class SeatRepository : ISeatRepository
    {
        private readonly ApplicationDbContext _context;

        public SeatRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Seat>> GetAllAsync(
            string? search,
            int? screenId,
            string? seatType,
            int pageNumber,
            int pageSize)
        {
            var query = _context.Seats
                .Include(s => s.Screen)
                .AsQueryable();

            // Show only active seats
            query = query.Where(s => s.IsActive);

            // Search by seat number
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(s =>
                    s.SeatNumber.Contains(search));
            }

            // Filter by screen
            if (screenId.HasValue)
            {
                query = query.Where(s =>
                    s.ScreenId == screenId.Value);
            }

            // Filter by seat type
            if (!string.IsNullOrWhiteSpace(seatType))
            {
                query = query.Where(s =>
                    s.SeatType == seatType);
            }

            return await query
                .OrderBy(s => s.SeatNumber)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Seat?> GetByIdAsync(int id)
        {
            return await _context.Seats
                .Include(s => s.Screen)
                .FirstOrDefaultAsync(s =>
                    s.Id == id && s.IsActive);
        }

        public async Task<Seat> AddAsync(Seat seat)
        {
            await _context.Seats.AddAsync(seat);
            await _context.SaveChangesAsync();

            return seat;
        }

        public async Task AddRangeAsync(IEnumerable<Seat> seats)
        {
            await _context.Seats.AddRangeAsync(seats);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Seat seat)
        {
            _context.Seats.Update(seat);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Seat seat)
        {
            _context.Seats.Remove(seat);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> SeatNumberExistsAsync(
            int screenId,
            string seatNumber,
            int? excludeSeatId = null)
        {
            var query = _context.Seats.Where(s =>
                s.ScreenId == screenId &&
                s.SeatNumber == seatNumber);

            if (excludeSeatId.HasValue)
            {
                query = query.Where(s =>
                    s.Id != excludeSeatId.Value);
            }

            return await query.AnyAsync();
        }
    }
}