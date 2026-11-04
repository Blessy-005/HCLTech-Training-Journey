
using System.Data;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<ApplicationUser> _userManager;

        public BookingService(
            IBookingRepository bookingRepository,
            IShowSeatRepository showSeatRepository,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _bookingRepository = bookingRepository;
            _showSeatRepository = showSeatRepository;
            _context = context;
            _userManager = userManager;
        }

        public async Task<List<BookingResponseDto>> GetAllAsync(
            BookingQueryDto query,
            string userId,
            bool isAdmin)
        {
            var bookings = await _bookingRepository.GetAllAsync(
                isAdmin ? query.CustomerEmail : null,
                query.Status,
                query.BookingDate,
                query.PageNumber,
                query.PageSize,
                isAdmin ? null : userId);

            return bookings.Select(MapToResponseDto).ToList();
        }

        public async Task<Booking?> GetResourceByIdAsync(int id)
        {
            return await _bookingRepository.GetByIdAsync(id);
        }

        public async Task<BookingResponseDto?> GetByIdAsync(
            int id,
            string userId,
            bool isAdmin)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);

            if (booking == null)
                return null;

            if (!isAdmin && booking.UserId != userId)
                return null;

            return MapToResponseDto(booking);
        }

        public async Task<BookingResponseDto> CreateAsync(
            CreateBookingDto dto,
            string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                throw new UnauthorizedAccessException(
                    "Authenticated user was not found.");

            var seatIds = dto.ShowSeatIds;

            if (seatIds == null || seatIds.Count == 0)
                throw new ArgumentException(
                    "Select at least one seat.");

            if (seatIds.Any(id => id <= 0))
                throw new ArgumentException("Invalid seat ID.");

            if (seatIds.Distinct().Count() != seatIds.Count)
                throw new ArgumentException(
                    "Duplicate seat IDs are not allowed.");

            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    IsolationLevel.Serializable);

            var showSeats =
                await _showSeatRepository.GetByIdsAsync(seatIds);

            if (showSeats.Count != seatIds.Count)
                throw new ArgumentException(
                    "One or more selected seats were not found.");

            if (showSeats.Any(ss => ss.Status != "Available"))
                throw new ArgumentException(
                    "One or more selected seats are already booked.");

            var showIds = showSeats
                .Select(ss => ss.ShowId)
                .Distinct()
                .ToList();

            if (showIds.Count != 1)
                throw new ArgumentException(
                    "All selected seats must belong to the same show.");

            var show = showSeats.First().Show;

            if (show == null ||
                !show.IsActive ||
                show.StartTime <= DateTime.Now)
            {
                throw new ArgumentException(
                    "This show is not available for booking.");
            }

            var totalAmount =
                showSeats.Sum(_ => show.TicketPrice);

            var booking = new Booking
            {
                UserId = user.Id,
                CustomerName = user.FullName,
                CustomerEmail = user.Email ?? string.Empty,
                BookingDate = DateTime.UtcNow,
                TotalAmount = totalAmount,
                Status = "Confirmed",

                BookingSeats = showSeats.Select(ss =>
                    new BookingSeat
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

            await transaction.CommitAsync();

            return MapToResponseDto(booking);
        }

        public async Task<bool> CancelAsync(
            int id,
            string userId,
            bool isAdmin)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    IsolationLevel.Serializable);

            var booking =
                await _bookingRepository.GetByIdAsync(id);

            if (booking == null)
                return false;

            if (!isAdmin && booking.UserId != userId)
                return false;

            if (booking.Status != "Confirmed")
                throw new ArgumentException(
                    "Only confirmed bookings can be cancelled.");

            foreach (var bookingSeat in booking.BookingSeats)
            {
                if (bookingSeat.ShowSeat == null)
                    throw new InvalidOperationException(
                        "Booking seat information is incomplete.");

                bookingSeat.ShowSeat.Status = "Available";
            }

            booking.Status = "Cancelled";

            await _bookingRepository.UpdateAsync(booking);

            await transaction.CommitAsync();

            return true;
        }

        private static BookingResponseDto MapToResponseDto(
            Booking booking)
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
                        SeatNumber =
                            bs.ShowSeat?.Seat?.SeatNumber
                            ?? string.Empty,
                        Price = bs.Price
                    }).ToList()
            };
        }
    }
}


