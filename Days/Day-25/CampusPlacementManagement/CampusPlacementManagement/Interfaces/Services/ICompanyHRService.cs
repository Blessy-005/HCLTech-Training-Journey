using CampusPlacementManagement.DTOs.CompanyHRs;

namespace CampusPlacementManagement.Interfaces.Services;

public interface ICompanyHRService
{
    Task<bool> RegisterAsync(RegisterCompanyHRDto dto);

    Task<IEnumerable<CompanyHRResponseDto>> GetAllAsync();

    Task<IEnumerable<CompanyHRResponseDto>> GetPendingAsync();

    Task<CompanyHRResponseDto?> GetByIdAsync(int id);

    Task<bool> ApproveAsync(int id, string approvedBy);

    Task<bool> RejectAsync(int id);

    Task<bool> SuspendAsync(int id);

    Task<CompanyHRResponseDto?> GetCurrentCompanyHRAsync(string userId);
}