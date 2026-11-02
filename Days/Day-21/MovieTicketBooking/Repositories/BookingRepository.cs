
using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _context;

        public BookingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Booking>> GetAllAsync(
            string? customerEmail,
            string? status,
            DateTime? bookingDate,
            int pageNumber,
            int pageSize,
            string? userId)
        {
            var query = _context.Bookings
                .Include(b => b.BookingSeats)
                    .ThenInclude(bs => bs.ShowSeat)
                        .ThenInclude(ss => ss!.Seat)
                .AsQueryable();

            // Restrict results to the logged-in customer.
            // Admin passes null to see all bookings.
            if (!string.IsNullOrWhiteSpace(userId))
            {
                query = query.Where(b => b.UserId == userId);
            }

            if (!string.IsNullOrWhiteSpace(customerEmail))
            {
                query = query.Where(b =>
                    b.CustomerEmail == customerEmail);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(b => b.Status == status);
            }

            if (bookingDate.HasValue)
            {
                var date = bookingDate.Value.Date;
                var nextDate = date.AddDays(1);

                query = query.Where(b =>
                    b.BookingDate >= date &&
                    b.BookingDate < nextDate);
            }

            return await query
                .OrderByDescending(b => b.BookingDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Booking?> GetByIdAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.BookingSeats)
                    .ThenInclude(bs => bs.ShowSeat)
                        .ThenInclude(ss => ss!.Seat)
                .Include(b => b.BookingSeats)
                    .ThenInclude(bs => bs.ShowSeat)
                        .ThenInclude(ss => ss!.Show)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Booking> AddAsync(Booking booking)
        {
            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();

            return booking;
        }

        public async Task UpdateAsync(Booking booking)
        {
            // The booking is already tracked by EF Core.
            await _context.SaveChangesAsync();
        }
    }
}
