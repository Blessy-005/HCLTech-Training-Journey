using MovieTicketBooking.DTOs.Movies;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;

        public MovieService(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;
        }

        // Get all movies with search, filters and pagination
        public async Task<IEnumerable<MovieResponseDto>> GetAllAsync(
            MovieQueryDto query)
        {
            var movies = await _movieRepository.GetAllAsync(
                query.Search,
                query.Language,
                query.Genre,
                query.PageNumber,
                query.PageSize);

            return movies.Select(movie => new MovieResponseDto
            {
                Id = movie.Id,
                Title = movie.Title,
                Description = movie.Description,
                Language = movie.Language,
                Genre = movie.Genre,
                DurationMinutes = movie.DurationMinutes,
                PosterUrl = movie.PosterUrl,
                ReleaseDate = movie.ReleaseDate,
                IsActive = movie.IsActive
            });
        }

        // Get a movie by ID
        public async Task<MovieResponseDto?> GetByIdAsync(int id)
        {
            var movie = await _movieRepository.GetByIdAsync(id);

            if (movie == null || !movie.IsActive)
            {
                return null;
            }

            return new MovieResponseDto
            {
                Id = movie.Id,
                Title = movie.Title,
                Description = movie.Description,
                Language = movie.Language,
                Genre = movie.Genre,
                DurationMinutes = movie.DurationMinutes,
                PosterUrl = movie.PosterUrl,
                ReleaseDate = movie.ReleaseDate,
                IsActive = movie.IsActive
            };
        }

        // Create a new movie
        public async Task<MovieResponseDto> CreateAsync(CreateMovieDto dto)
        {
            var movie = new Movie
            {
                Title = dto.Title,
                Description = dto.Description,
                Language = dto.Language,
                Genre = dto.Genre,
                DurationMinutes = dto.DurationMinutes,
                PosterUrl = dto.PosterUrl,
                ReleaseDate = dto.ReleaseDate,
                IsActive = true
            };

            var createdMovie = await _movieRepository.AddAsync(movie);

            return new MovieResponseDto
            {
                Id = createdMovie.Id,
                Title = createdMovie.Title,
                Description = createdMovie.Description,
                Language = createdMovie.Language,
                Genre = createdMovie.Genre,
                DurationMinutes = createdMovie.DurationMinutes,
                PosterUrl = createdMovie.PosterUrl,
                ReleaseDate = createdMovie.ReleaseDate,
                IsActive = createdMovie.IsActive
            };
        }

        // Update an existing movie
        public async Task<bool> UpdateAsync(int id, UpdateMovieDto dto)
        {
            var movie = await _movieRepository.GetByIdAsync(id);

            if (movie == null || !movie.IsActive)
            {
                return false;
            }

            movie.Title = dto.Title;
            movie.Description = dto.Description;
            movie.Language = dto.Language;
            movie.Genre = dto.Genre;
            movie.DurationMinutes = dto.DurationMinutes;
            movie.PosterUrl = dto.PosterUrl;
            movie.ReleaseDate = dto.ReleaseDate;
            movie.IsActive = dto.IsActive;

            await _movieRepository.UpdateAsync(movie);

            return true;
        }

        // Deactivate a movie instead of permanently deleting it
        public async Task<bool> DeleteAsync(int id)
        {
            var movie = await _movieRepository.GetByIdAsync(id);

            if (movie == null || !movie.IsActive)
            {
                return false;
            }

            movie.IsActive = false;

            await _movieRepository.UpdateAsync(movie);

            return true;
        }
    }
}