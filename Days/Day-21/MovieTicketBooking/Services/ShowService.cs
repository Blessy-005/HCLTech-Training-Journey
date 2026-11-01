using MovieTicketBooking.DTOs.Shows;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services
{
    public class ShowService : IShowService
    {
        private readonly IShowRepository _showRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IScreenRepository _screenRepository;
        private readonly ISeatRepository _seatRepository;
        private readonly IShowSeatRepository _showSeatRepository;

        public ShowService(
    IShowRepository showRepository,
    IMovieRepository movieRepository,
    IScreenRepository screenRepository,
    ISeatRepository seatRepository,
    IShowSeatRepository showSeatRepository)
        {
            _showRepository = showRepository;
            _movieRepository = movieRepository;
            _screenRepository = screenRepository;
            _seatRepository = seatRepository;
            _showSeatRepository = showSeatRepository;
        }

        public async Task<List<ShowResponseDto>> GetAllAsync(ShowQueryDto query)
        {
            var shows = await _showRepository.GetAllAsync(
                query.MovieId,
                query.ScreenId,
                query.ShowDate,
                query.IsActive,
                query.PageNumber,
                query.PageSize);

            return shows.Select(MapToResponseDto).ToList();
        }

        public async Task<ShowResponseDto?> GetByIdAsync(int id)
        {
            var show = await _showRepository.GetByIdAsync(id);

            if (show == null)
                return null;

            return MapToResponseDto(show);
        }

        public async Task<ShowResponseDto> CreateAsync(CreateShowDto dto)
        {
            if (dto.EndTime <= dto.StartTime)
                throw new ArgumentException("End time must be after start time.");

            var movie = await _movieRepository.GetByIdAsync(dto.MovieId);

            if (movie == null)
                throw new ArgumentException("Movie not found or inactive.");

            var screen = await _screenRepository.GetByIdAsync(dto.ScreenId);

            if (screen == null)
                throw new ArgumentException("Screen not found or inactive.");

            var expectedEndTime = dto.StartTime.AddMinutes(movie.DurationMinutes);

            if (dto.EndTime != expectedEndTime)
                throw new ArgumentException(
                    "End time must match the movie duration.");

            var hasOverlap = await _showRepository.HasOverlappingShowAsync(
                dto.ScreenId,
                dto.StartTime,
                dto.EndTime);

            if (hasOverlap)
                throw new ArgumentException(
                    "Another show is already scheduled on this screen during this time.");

            var show = new Show
            {
                MovieId = dto.MovieId,
                ScreenId = dto.ScreenId,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                TicketPrice = dto.TicketPrice,
                IsActive = true
            };

            var createdShow = await _showRepository.AddAsync(show);

            // Get all active physical seats in this screen
            var seats = await _seatRepository.GetActiveByScreenIdAsync(
                createdShow.ScreenId);

            // Create an availability record for every physical seat
            var showSeats = seats.Select(seat => new ShowSeat
            {
                ShowId = createdShow.Id,
                SeatId = seat.Id,
                Status = "Available"
            }).ToList();

            // Save all ShowSeat records
            if (showSeats.Any())
            {
                await _showSeatRepository.AddRangeAsync(showSeats);
            }

            createdShow.Movie = movie;
            createdShow.Screen = screen;

            return MapToResponseDto(createdShow);
        }

        public async Task<bool> UpdateAsync(int id, UpdateShowDto dto)
        {
            var show = await _showRepository.GetByIdAsync(id);

            if (show == null)
                return false;

            if (dto.EndTime <= dto.StartTime)
                throw new ArgumentException("End time must be after start time.");

            var movie = await _movieRepository.GetByIdAsync(dto.MovieId);

            if (movie == null)
                throw new ArgumentException("Movie not found or inactive.");

            var screen = await _screenRepository.GetByIdAsync(dto.ScreenId);

            if (screen == null)
                throw new ArgumentException("Screen not found or inactive.");

            var expectedEndTime = dto.StartTime.AddMinutes(movie.DurationMinutes);

            if (dto.EndTime != expectedEndTime)
                throw new ArgumentException(
                    "End time must match the movie duration.");

            var hasOverlap = await _showRepository.HasOverlappingShowAsync(
                dto.ScreenId,
                dto.StartTime,
                dto.EndTime,
                id);

            if (hasOverlap)
                throw new ArgumentException(
                    "Another show is already scheduled on this screen during this time.");

            show.MovieId = dto.MovieId;
            show.ScreenId = dto.ScreenId;
            show.StartTime = dto.StartTime;
            show.EndTime = dto.EndTime;
            show.TicketPrice = dto.TicketPrice;
            show.IsActive = dto.IsActive;

            await _showRepository.UpdateAsync(show);

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var show = await _showRepository.GetByIdAsync(id);

            if (show == null)
                return false;

            show.IsActive = false;

            await _showRepository.UpdateAsync(show);

            return true;
        }

        private static ShowResponseDto MapToResponseDto(Show show)
        {
            return new ShowResponseDto
            {
                Id = show.Id,
                MovieId = show.MovieId,
                MovieTitle = show.Movie?.Title ?? string.Empty,
                ScreenId = show.ScreenId,
                ScreenName = show.Screen?.Name ?? string.Empty,
                TheatreName = show.Screen?.Theatre?.Name ?? string.Empty,
                StartTime = show.StartTime,
                EndTime = show.EndTime,
                TicketPrice = show.TicketPrice,
                IsActive = show.IsActive
            };
        }
    }
}