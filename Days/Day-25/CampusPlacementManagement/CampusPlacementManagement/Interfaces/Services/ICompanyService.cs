using CampusPlacementManagement.DTOs.Companies;

namespace CampusPlacementManagement.Interfaces.Services;

public interface ICompanyService
{
    Task<CompanyResponseDto> RegisterAsync(RegisterCompanyDto dto);

    Task<IEnumerable<CompanyResponseDto>> GetAllAsync();

    Task<IEnumerable<CompanyResponseDto>> GetPendingAsync();

    Task<CompanyResponseDto?> GetByIdAsync(
        int id,
        string userId,
        bool isSuperAdmin);

    Task<CompanyResponseDto?> UpdateAsync(
        int id,
        UpdateCompanyDto dto,
        string userId,
        bool isSuperAdmin);

    Task<bool> ApproveAsync(int id);
    Task<bool> RejectAsync(int id);
    Task<bool> SuspendAsync(int id);
}
