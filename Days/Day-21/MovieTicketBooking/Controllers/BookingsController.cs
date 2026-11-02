
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs.Bookings;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] BookingQueryDto query)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = User.IsInRole("Admin");

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var bookings = await _bookingService.GetAllAsync(
                query, userId, isAdmin);

            return Ok(bookings);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = User.IsInRole("Admin");

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var booking = await _bookingService.GetByIdAsync(
                id, userId, isAdmin);

            if (booking == null)
                return NotFound("Booking not found.");

            return Ok(booking);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBookingDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var booking = await _bookingService.CreateAsync(dto, userId);

            return CreatedAtAction(
                nameof(GetById),
                new { id = booking.Id },
                booking);
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = User.IsInRole("Admin");

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _bookingService.CancelAsync(
                id, userId, isAdmin);

            if (!result)
                return NotFound("Booking not found.");

            return Ok("Booking cancelled successfully.");
        }
    }
}

