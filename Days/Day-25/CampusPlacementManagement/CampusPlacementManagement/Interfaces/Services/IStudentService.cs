using CampusPlacementManagement.DTOs.Students;

namespace CampusPlacementManagement.Interfaces.Services;

public interface IStudentService
{
    Task<StudentResponseDto> EnrollAsync(RegisterStudentDto dto, string placementOfficerUserId);

    Task<IEnumerable<StudentResponseDto>> GetAllAsync(string placementOfficerUserId);

    Task<IEnumerable<StudentResponseDto>> GetPendingAsync(string placementOfficerUserId);

    Task<StudentResponseDto> GetByIdAsync(int id, string placementOfficerUserId);

    Task<StudentResponseDto> ApproveAsync(int id, string placementOfficerUserId);

    Task<StudentResponseDto> RejectAsync(int id, string placementOfficerUserId);

    Task<StudentResponseDto> SuspendAsync(int id, string placementOfficerUserId);

    Task<StudentResponseDto> GetCurrentStudentAsync(string userId);
}
