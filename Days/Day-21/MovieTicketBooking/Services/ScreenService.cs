using MovieTicketBooking.DTOs.Screens;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services
{
    public class ScreenService : IScreenService
    {
        private readonly IScreenRepository _screenRepository;
        private readonly ITheatreRepository _theatreRepository;

        public ScreenService(
            IScreenRepository screenRepository,
            ITheatreRepository theatreRepository)
        {
            _screenRepository = screenRepository;
            _theatreRepository = theatreRepository;
        }

        public async Task<IEnumerable<ScreenResponseDto>> GetAllAsync(
            ScreenQueryDto query)
        {
            var screens = await _screenRepository.GetAllAsync(
                query.Search,
                query.TheatreId,
                query.PageNumber,
                query.PageSize);

            return screens.Select(MapToResponseDto);
        }

        public async Task<ScreenResponseDto?> GetByIdAsync(int id)
        {
            var screen = await _screenRepository.GetByIdAsync(id);

            if (screen == null)
            {
                return null;
            }

            return MapToResponseDto(screen);
        }

        public async Task<ScreenResponseDto> CreateAsync(
            CreateScreenDto dto)
        {
            // Check whether the theatre exists and is active.
            var theatre = await _theatreRepository.GetByIdAsync(dto.TheatreId);

            if (theatre == null)
            {
                throw new ArgumentException(
                    "The selected theatre does not exist or is inactive.");
            }

            var screen = new Screen
            {
                Name = dto.Name,
                TheatreId = dto.TheatreId,
                TotalSeats = dto.TotalSeats,
                IsActive = true
            };

            var createdScreen = await _screenRepository.AddAsync(screen);

            // Load the theatre navigation property for the response.
            createdScreen.Theatre = theatre;

            return MapToResponseDto(createdScreen);
        }

        public async Task<bool> UpdateAsync(
            int id,
            UpdateScreenDto dto)
        {
            var screen = await _screenRepository.GetByIdAsync(id);

            if (screen == null)
            {
                return false;
            }

            var theatre = await _theatreRepository.GetByIdAsync(dto.TheatreId);

            if (theatre == null)
            {
                throw new ArgumentException(
                    "The selected theatre does not exist or is inactive.");
            }

            screen.Name = dto.Name;
            screen.TheatreId = dto.TheatreId;
            screen.TotalSeats = dto.TotalSeats;
            screen.IsActive = dto.IsActive;

            await _screenRepository.UpdateAsync(screen);

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var screen = await _screenRepository.GetByIdAsync(id);

            if (screen == null)
            {
                return false;
            }

            // Soft delete: preserve the database record.
            screen.IsActive = false;

            await _screenRepository.UpdateAsync(screen);

            return true;
        }

        private static ScreenResponseDto MapToResponseDto(Screen screen)
        {
            return new ScreenResponseDto
            {
                Id = screen.Id,
                Name = screen.Name,
                TheatreId = screen.TheatreId,
                TheatreName = screen.Theatre?.Name ?? string.Empty,
                TotalSeats = screen.TotalSeats,
                IsActive = screen.IsActive
            };
        }
    }
}