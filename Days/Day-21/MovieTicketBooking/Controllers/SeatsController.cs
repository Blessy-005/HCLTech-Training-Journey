using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs.Seats;
using MovieTicketBooking.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
namespace MovieTicketBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeatsController : ControllerBase
    {
        private readonly ISeatService _seatService;

        public SeatsController(ISeatService seatService)
        {
            _seatService = seatService;
        }

        // GET: api/seats
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] SeatQueryDto query)
        {
            var seats = await _seatService.GetAllAsync(query);

            return Ok(seats);
        }

        // GET: api/seats/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var seat = await _seatService.GetByIdAsync(id);

            if (seat == null)
            {
                return NotFound(new
                {
                    message = "Seat not found."
                });
            }

            return Ok(seat);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateSeatDto dto)
        {
            var seat = await _seatService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = seat.Id },
                seat);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("bulk")]
        public async Task<IActionResult> CreateBulk(
            [FromBody] IEnumerable<CreateSeatDto> seats)
        {
            var createdSeats = await _seatService.CreateBulkAsync(seats);

            return Ok(createdSeats);
        }

        // PUT: api/seats/1
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateSeatDto dto)
        {
            var updated = await _seatService.UpdateAsync(id, dto);

            if (!updated)
            {
                return NotFound(new
                {
                    message = "Seat not found."
                });
            }

            return NoContent();
        }

        // DELETE: api/seats/1
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _seatService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound(new
                {
                    message = "Seat not found."
                });
            }

            return NoContent();
        }
    }
}