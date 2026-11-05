using CampusPlacementManagement.DTOs.Applications;

namespace CampusPlacementManagement.Interfaces.Services;

public interface IApplicationStatusHistoryService
{
    Task<bool> UpdateStatusAsync(
        int applicationId,
        UpdateApplicationStatusDto dto,
        string updatedByUserId);

    Task<List<ApplicationStatusHistoryDto>> GetHistoryAsync(
        int applicationId,
        string requestedByUserId);
}
