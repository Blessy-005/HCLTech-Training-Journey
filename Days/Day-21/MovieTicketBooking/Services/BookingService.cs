using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.DTOs.Bookings;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IShowSeatRepository _showSeatRepository;
        private readonly ApplicationDbContext _context;

        public BookingService(
            IBookingRepository bookingRepository,
            IShowSeatRepository showSeatRepository,
            ApplicationDbContext context)
        {
            _bookingRepository = bookingRepository;
            _showSeatRepository = showSeatRepository;
            _context = context;
        }

        public async Task<List<BookingResponseDto>> GetAllAsync(
            BookingQueryDto query)
        {
            var bookings = await _bookingRepository.GetAllAsync(
                query.CustomerEmail,
                query.Status,
                query.BookingDate,
                query.PageNumber,
                query.PageSize);

            return bookings.Select(MapToResponseDto).ToList();
        }

        public async Task<BookingResponseDto?> GetByIdAsync(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);

            if (booking == null)
                return null;

            return MapToResponseDto(booking);
        }

        public async Task<BookingResponseDto> CreateAsync(
            CreateBookingDto dto)
        {
            var seatIds = dto.ShowSeatIds;

            if (seatIds == null || seatIds.Count == 0)
                throw new ArgumentException("Select at least one seat.");

            if (seatIds.Any(id => id <= 0))
                throw new ArgumentException("Invalid seat ID.");

            if (seatIds.Distinct().Count() != seatIds.Count)
                throw new ArgumentException("Duplicate seat IDs are not allowed.");

            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            var showSeats = await _showSeatRepository.GetByIdsAsync(seatIds);

            if (showSeats.Count != seatIds.Count)
                throw new ArgumentException("One or more selected seats were not found.");

            if (showSeats.Any(ss => ss.Status != "Available"))
                throw new ArgumentException("One or more selected seats are already booked.");

            var showIds = showSeats.Select(ss => ss.ShowId).Distinct().ToList();

            if (showIds.Count != 1)
                throw new ArgumentException("All selected seats must belong to the same show.");

            var show = showSeats.First().Show;

            if (show == null || !show.IsActive || show.StartTime <= DateTime.Now)
                throw new ArgumentException("This show is not available for booking.");

            var totalAmount = showSeats.Sum(_ => show.TicketPrice);

            var booking = new Booking
            {
                CustomerName = dto.CustomerName.Trim(),
                CustomerEmail = dto.CustomerEmail.Trim(),
                BookingDate = DateTime.UtcNow,
                TotalAmount = totalAmount,
                Status = "Confirmed",
                BookingSeats = showSeats.Select(ss => new BookingSeat
                {
                    ShowSeatId = ss.Id,
                    Price = show.TicketPrice
                }).ToList()
            };

            foreach (var showSeat in showSeats)
            {
                showSeat.Status = "Booked";
            }

            await _bookingRepository.AddAsync(booking);

            foreach (var showSeat in showSeats)
            {
                await _showSeatRepository.UpdateAsync(showSeat);
            }

            await transaction.CommitAsync();

            return MapToResponseDto(booking);
        }

        public async Task<bool> CancelAsync(int id)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            var booking = await _bookingRepository.GetByIdAsync(id);

            if (booking == null)
                return false;

            if (booking.Status == "Cancelled")
                throw new ArgumentException("This booking is already cancelled.");

            foreach (var bookingSeat in booking.BookingSeats)
            {
                bookingSeat.ShowSeat.Status = "Available";
            }

            booking.Status = "Cancelled";

            await _bookingRepository.UpdateAsync(booking);

            foreach (var bookingSeat in booking.BookingSeats)
            {
                await _showSeatRepository.UpdateAsync(bookingSeat.ShowSeat);
            }

            await transaction.CommitAsync();

            return true;
        }

        private static BookingResponseDto MapToResponseDto(Booking booking)
        {
            return new BookingResponseDto
            {
                Id = booking.Id,
                CustomerName = booking.CustomerName,
                CustomerEmail = booking.CustomerEmail,
                BookingDate = booking.BookingDate,
                TotalAmount = booking.TotalAmount,
                Status = booking.Status,
                Seats = booking.BookingSeats.Select(bs =>
                    new BookingSeatResponseDto
                    {
                        Id = bs.Id,
                        ShowSeatId = bs.ShowSeatId,
                        SeatNumber = bs.ShowSeat?.Seat?.SeatNumber ?? string.Empty,
                        Price = bs.Price
                    }).ToList()
            };
        }
    }
}