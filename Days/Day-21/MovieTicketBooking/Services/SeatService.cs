using MovieTicketBooking.DTOs.Seats;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services
{
    public class SeatService : ISeatService
    {
        private readonly ISeatRepository _seatRepository;
        private readonly IScreenRepository _screenRepository;

        public SeatService(
            ISeatRepository seatRepository,
            IScreenRepository screenRepository)
        {
            _seatRepository = seatRepository;
            _screenRepository = screenRepository;
        }

        public async Task<IEnumerable<SeatResponseDto>> GetAllAsync(
            SeatQueryDto query)
        {
            var seats = await _seatRepository.GetAllAsync(
                query.Search,
                query.ScreenId,
                query.SeatType,
                query.PageNumber,
                query.PageSize);

            return seats.Select(MapToResponseDto);
        }

        public async Task<SeatResponseDto?> GetByIdAsync(int id)
        {
            var seat = await _seatRepository.GetByIdAsync(id);

            if (seat == null)
            {
                return null;
            }

            return MapToResponseDto(seat);
        }

        public async Task<SeatResponseDto> CreateAsync(
            CreateSeatDto dto)
        {
            var screen = await _screenRepository.GetByIdAsync(dto.ScreenId);

            if (screen == null)
            {
                throw new ArgumentException(
                    "The selected screen does not exist or is inactive.");
            }

            bool exists = await _seatRepository.SeatNumberExistsAsync(
                dto.ScreenId,
                dto.SeatNumber);

            if (exists)
            {
                throw new ArgumentException(
                    "This seat number already exists in the selected screen.");
            }

            var seat = new Seat
            {
                SeatNumber = dto.SeatNumber,
                SeatType = dto.SeatType,
                ScreenId = dto.ScreenId,
                IsActive = true,
                Screen = screen
            };

            var createdSeat = await _seatRepository.AddAsync(seat);

            return MapToResponseDto(createdSeat);
        }

        public async Task<IEnumerable<SeatResponseDto>> CreateBulkAsync(
            IEnumerable<CreateSeatDto> seats)
        {
            var seatList = seats.ToList();

            if (seatList.Count == 0)
            {
                throw new ArgumentException(
                    "At least one seat must be provided.");
            }

            // Check duplicate screen and seat-number combinations
            var duplicates = seatList
                .GroupBy(s => new
                {
                    s.ScreenId,
                    SeatNumber = s.SeatNumber.Trim()
                })
                .Any(g => g.Count() > 1);

            if (duplicates)
            {
                throw new ArgumentException(
                    "Duplicate seat numbers were found in the request.");
            }

            // Validate all screens
            var screens = new Dictionary<int, Screen>();

            foreach (var screenId in seatList.Select(s => s.ScreenId).Distinct())
            {
                var screen = await _screenRepository.GetByIdAsync(screenId);

                if (screen == null)
                {
                    throw new ArgumentException(
                        $"Screen with ID {screenId} does not exist or is inactive.");
                }

                screens.Add(screenId, screen);
            }

            // Validate existing seat numbers before inserting
            foreach (var dto in seatList)
            {
                bool exists = await _seatRepository.SeatNumberExistsAsync(
                    dto.ScreenId,
                    dto.SeatNumber);

                if (exists)
                {
                    throw new ArgumentException(
                        $"Seat number {dto.SeatNumber} already exists in screen {dto.ScreenId}.");
                }
            }

            // Prepare all seats
            var newSeats = seatList.Select(dto => new Seat
            {
                SeatNumber = dto.SeatNumber.Trim(),
                SeatType = dto.SeatType,
                ScreenId = dto.ScreenId,
                IsActive = true,
                Screen = screens[dto.ScreenId]
            }).ToList();

            // Save all seats together
            await _seatRepository.AddRangeAsync(newSeats);

            return newSeats.Select(MapToResponseDto);
        }

        public async Task<bool> UpdateAsync(
            int id,
            UpdateSeatDto dto)
        {
            var seat = await _seatRepository.GetByIdAsync(id);

            if (seat == null)
            {
                return false;
            }

            var screen = await _screenRepository.GetByIdAsync(dto.ScreenId);

            if (screen == null)
            {
                throw new ArgumentException(
                    "The selected screen does not exist or is inactive.");
            }

            bool exists = await _seatRepository.SeatNumberExistsAsync(
                dto.ScreenId,
                dto.SeatNumber,
                id);

            if (exists)
            {
                throw new ArgumentException(
                    "This seat number already exists in the selected screen.");
            }

            seat.SeatNumber = dto.SeatNumber;
            seat.SeatType = dto.SeatType;
            seat.ScreenId = dto.ScreenId;
            seat.IsActive = dto.IsActive;
            seat.Screen = screen;

            await _seatRepository.UpdateAsync(seat);

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var seat = await _seatRepository.GetByIdAsync(id);

            if (seat == null)
            {
                return false;
            }

            // Soft delete
            seat.IsActive = false;

            await _seatRepository.UpdateAsync(seat);

            return true;
        }

        private static SeatResponseDto MapToResponseDto(Seat seat)
        {
            return new SeatResponseDto
            {
                Id = seat.Id,
                SeatNumber = seat.SeatNumber,
                SeatType = seat.SeatType,
                ScreenId = seat.ScreenId,
                ScreenName = seat.Screen?.Name ?? string.Empty,
                IsActive = seat.IsActive
            };
        }
    }
}