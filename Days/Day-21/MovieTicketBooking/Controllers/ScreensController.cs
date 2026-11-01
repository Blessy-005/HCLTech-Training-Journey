using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs.Screens;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScreensController : ControllerBase
    {
        private readonly IScreenService _screenService;

        public ScreensController(IScreenService screenService)
        {
            _screenService = screenService;
        }

        // GET: api/screens
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] ScreenQueryDto query)
        {
            var screens = await _screenService.GetAllAsync(query);

            return Ok(screens);
        }

        // GET: api/screens/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var screen = await _screenService.GetByIdAsync(id);

            if (screen == null)
            {
                return NotFound(new
                {
                    message = "Screen not found."
                });
            }

            return Ok(screen);
        }

        // POST: api/screens
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateScreenDto dto)
        {
            var screen = await _screenService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = screen.Id },
                screen);
        }

        // PUT: api/screens/1
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateScreenDto dto)
        {
            var updated = await _screenService.UpdateAsync(id, dto);

            if (!updated)
            {
                return NotFound(new
                {
                    message = "Screen not found."
                });
            }

            return NoContent();
        }

        // DELETE: api/screens/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _screenService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound(new
                {
                    message = "Screen not found."
                });
            }

            return NoContent();
        }
    }
}