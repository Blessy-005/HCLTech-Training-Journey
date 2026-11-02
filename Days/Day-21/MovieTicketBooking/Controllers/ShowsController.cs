using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs.Shows;
using MovieTicketBooking.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
namespace MovieTicketBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShowsController : ControllerBase
    {
        private readonly IShowService _showService;

        public ShowsController(IShowService showService)
        {
            _showService = showService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllShows([FromQuery] ShowQueryDto query)
        {
            var shows = await _showService.GetAllAsync(query);
            return Ok(shows);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetShowById(int id)
        {
            var show = await _showService.GetByIdAsync(id);

            if (show == null)
                return NotFound(new { message = "Show not found." });

            return Ok(show);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateShow(CreateShowDto dto)
        {
            var show = await _showService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetShowById),
                new { id = show.Id },
                show);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateShow(int id, UpdateShowDto dto)
        {
            var updated = await _showService.UpdateAsync(id, dto);

            if (!updated)
                return NotFound(new { message = "Show not found." });

            return Ok(new { message = "Show updated successfully." });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShow(int id)
        {
            var deleted = await _showService.DeleteAsync(id);

            if (!deleted)
                return NotFound(new { message = "Show not found." });

            return Ok(new { message = "Show deactivated successfully." });
        }
    }
}