using CampusPlacementManagement.Models.Entities;

namespace CampusPlacementManagement.Interfaces.Services;

public interface IJwtTokenService
{
    string GenerateToken(ApplicationUser user, IList<string> roles);
}
