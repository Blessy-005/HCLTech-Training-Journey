using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs.Theatres;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TheatresController : ControllerBase
    {
        private readonly ITheatreService _theatreService;

        public TheatresController(ITheatreService theatreService)
        {
            _theatreService = theatreService;
        }

        // GET: api/theatres
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] TheatreQueryDto query)
        {
            var theatres = await _theatreService.GetAllAsync(query);

            return Ok(theatres);
        }

        // GET: api/theatres/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var theatre = await _theatreService.GetByIdAsync(id);

            if (theatre == null)
            {
                return NotFound(new
                {
                    message = "Theatre not found."
                });
            }

            return Ok(theatre);
        }

        // POST: api/theatres
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateTheatreDto dto)
        {
            var theatre = await _theatreService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = theatre.Id },
                theatre);
        }

        // PUT: api/theatres/1
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateTheatreDto dto)
        {
            var updated = await _theatreService.UpdateAsync(id, dto);

            if (!updated)
            {
                return NotFound(new
                {
                    message = "Theatre not found."
                });
            }

            return NoContent();
        }

        // DELETE: api/theatres/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _theatreService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound(new
                {
                    message = "Theatre not found."
                });
            }

            return NoContent();
        }
    }
}