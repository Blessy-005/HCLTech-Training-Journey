using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs.Movies;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MoviesController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        // GET: api/movies
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] MovieQueryDto query)
        {
            var movies = await _movieService.GetAllAsync(query);

            return Ok(movies);
        }

        // GET: api/movies/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var movie = await _movieService.GetByIdAsync(id);

            if (movie == null)
            {
                return NotFound(new
                {
                    message = $"Movie with ID {id} was not found."
                });
            }

            return Ok(movie);
        }

        // POST: api/movies
        [HttpPost]
        public async Task<IActionResult> Create(CreateMovieDto dto)
        {
            var movie = await _movieService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = movie.Id },
                movie);
        }

        // PUT: api/movies/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            UpdateMovieDto dto)
        {
            var result = await _movieService.UpdateAsync(id, dto);

            if (!result)
            {
                return NotFound(new
                {
                    message = $"Movie with ID {id} was not found."
                });
            }

            return NoContent();
        }

        // DELETE: api/movies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _movieService.DeleteAsync(id);

            if (!result)
            {
                return NotFound(new
                {
                    message = $"Movie with ID {id} was not found."
                });
            }

            return NoContent();
        }
    }
}