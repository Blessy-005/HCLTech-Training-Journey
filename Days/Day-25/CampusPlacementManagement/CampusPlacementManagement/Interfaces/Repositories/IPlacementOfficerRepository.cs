using CampusPlacementManagement.Models.Entities;

namespace CampusPlacementManagement.Interfaces.Repositories;

public interface IPlacementOfficerRepository
{
    Task<PlacementOfficer?> GetByIdAsync(int id);

    Task<PlacementOfficer?> GetByUserIdAsync(string userId);

    Task<IEnumerable<PlacementOfficer>> GetAllAsync();

    Task<IEnumerable<PlacementOfficer>> GetPendingAsync();

    Task AddAsync(PlacementOfficer placementOfficer);

    Task UpdateAsync(PlacementOfficer placementOfficer);
}
