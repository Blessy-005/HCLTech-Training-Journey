using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs.ShowSeats;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShowSeatsController : ControllerBase
    {
        private readonly IShowSeatService _showSeatService;

        public ShowSeatsController(IShowSeatService showSeatService)
        {
            _showSeatService = showSeatService;
        }

        [HttpGet]
        public async Task<IActionResult> GetShowSeats(
            [FromQuery] ShowSeatQueryDto query)
        {
            var showSeats = await _showSeatService.GetByShowIdAsync(query);

            return Ok(showSeats);
        }
    }
}