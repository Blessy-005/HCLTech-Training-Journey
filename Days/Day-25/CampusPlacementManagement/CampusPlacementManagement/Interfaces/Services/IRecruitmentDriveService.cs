using CampusPlacementManagement.DTOs.RecruitmentDrives;

namespace CampusPlacementManagement.Interfaces.Services;

public interface IRecruitmentDriveService
{
    Task<RecruitmentDriveResponseDto> CreateAsync(
        CreateRecruitmentDriveDto dto, string userId);

    Task<IEnumerable<RecruitmentDriveResponseDto>> GetVisibleAsync(
        string userId, string role);

    Task<RecruitmentDriveResponseDto?> GetByIdAsync(
        int driveId, string userId, string role);

    Task<RecruitmentDriveResponseDto?> UpdateAsync(
        int driveId, UpdateRecruitmentDriveDto dto, string userId);

    Task<bool> CancelAsync(int driveId, string userId);
}
