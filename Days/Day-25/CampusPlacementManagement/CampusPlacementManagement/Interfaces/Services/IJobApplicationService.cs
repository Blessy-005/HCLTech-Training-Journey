using CampusPlacementManagement.DTOs.Applications;

namespace CampusPlacementManagement.Interfaces.Services;

public interface IJobApplicationService
{
    Task<JobApplicationResponseDto> ApplyAsync(
        ApplyJobDto dto, string userId);

    Task<IEnumerable<JobApplicationResponseDto>> GetMyApplicationsAsync(
        string userId);

    Task<JobApplicationResponseDto> GetByIdAsync(
        int applicationId, string userId, string role);

    Task<IEnumerable<JobApplicationResponseDto>> GetForJobAsync(
        int jobId, string userId, string role);

    Task WithdrawAsync(
        int applicationId, string userId);
}
