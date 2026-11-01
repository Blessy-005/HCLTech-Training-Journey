using MovieTicketBooking.DTOs.Theatres;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services
{
    public class TheatreService : ITheatreService
    {
        private readonly ITheatreRepository _theatreRepository;

        public TheatreService(ITheatreRepository theatreRepository)
        {
            _theatreRepository = theatreRepository;
        }

        public async Task<IEnumerable<TheatreResponseDto>> GetAllAsync(
            TheatreQueryDto query)
        {
            var theatres = await _theatreRepository.GetAllAsync(
                query.Search,
                query.City,
                query.PageNumber,
                query.PageSize);

            return theatres.Select(t => new TheatreResponseDto
            {
                Id = t.Id,
                Name = t.Name,
                Address = t.Address,
                City = t.City,
                IsActive = t.IsActive
            });
        }

        public async Task<TheatreResponseDto?> GetByIdAsync(int id)
        {
            var theatre = await _theatreRepository.GetByIdAsync(id);

            if (theatre == null)
            {
                return null;
            }

            return new TheatreResponseDto
            {
                Id = theatre.Id,
                Name = theatre.Name,
                Address = theatre.Address,
                City = theatre.City,
                IsActive = theatre.IsActive
            };
        }

        public async Task<TheatreResponseDto> CreateAsync(
            CreateTheatreDto dto)
        {
            var theatre = new Theatre
            {
                Name = dto.Name,
                Address = dto.Address,
                City = dto.City,
                IsActive = true
            };

            var createdTheatre = await _theatreRepository.AddAsync(theatre);

            return new TheatreResponseDto
            {
                Id = createdTheatre.Id,
                Name = createdTheatre.Name,
                Address = createdTheatre.Address,
                City = createdTheatre.City,
                IsActive = createdTheatre.IsActive
            };
        }

        public async Task<bool> UpdateAsync(
            int id,
            UpdateTheatreDto dto)
        {
            var theatre = await _theatreRepository.GetByIdAsync(id);

            if (theatre == null)
            {
                return false;
            }

            theatre.Name = dto.Name;
            theatre.Address = dto.Address;
            theatre.City = dto.City;
            theatre.IsActive = dto.IsActive;

            await _theatreRepository.UpdateAsync(theatre);

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var theatre = await _theatreRepository.GetByIdAsync(id);

            if (theatre == null)
            {
                return false;
            }

            // Soft delete: keep the record, but deactivate it.
            theatre.IsActive = false;

            await _theatreRepository.UpdateAsync(theatre);

            return true;
        }
    }
}