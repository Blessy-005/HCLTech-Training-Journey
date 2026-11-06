using CampusPlacementManagement.DTOs.Jobs;

namespace CampusPlacementManagement.Interfaces.Services;

public interface IJobOpeningService
{
    Task<JobOpeningResponseDto> CreateAsync(
        CreateJobOpeningDto dto,
        string userId);

    Task<IEnumerable<JobOpeningResponseDto>> GetMyJobsAsync(
        string userId);

    Task<JobOpeningResponseDto?> GetByIdAsync(
        int jobId,
        string userId);

    Task<JobOpeningResponseDto?> UpdateAsync(
        int jobId,
        UpdateJobOpeningDto dto,
        string userId);

    Task<bool> PublishAsync(int jobId, string userId);

    Task<bool> CloseAsync(int jobId, string userId);

    Task<bool> SetTargetCollegesAsync(
        int jobId,
        SetJobTargetCollegesDto dto,
        string userId);

    Task<IEnumerable<int>?> GetTargetCollegesAsync(
        int jobId,
        string userId);

    Task<bool> RemoveTargetCollegeAsync(
        int jobId,
        int collegeId,
        string userId);
}
