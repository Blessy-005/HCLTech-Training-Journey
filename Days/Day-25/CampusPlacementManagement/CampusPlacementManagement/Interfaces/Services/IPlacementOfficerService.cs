using CampusPlacementManagement.DTOs.PlacementOfficers;

namespace CampusPlacementManagement.Interfaces.Services;

public interface IPlacementOfficerService
{
    Task<bool> RegisterAsync(RegisterPlacementOfficerDto dto);

    Task<IEnumerable<object>> GetAllAsync();

    Task<IEnumerable<object>> GetPendingAsync();

    Task<object?> GetByIdAsync(int id);

    Task<bool> ApproveAsync(int id, string approvedBy);

    Task<bool> RejectAsync(int id);

    Task<bool> SuspendAsync(int id);

    Task<object?> GetCurrentOfficerAsync(string userId);
}
