using CampusPlacementManagement.DTOs.Authentication;

namespace CampusPlacementManagement.Interfaces.Services;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginDto dto);
}
