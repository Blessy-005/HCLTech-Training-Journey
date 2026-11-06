using CampusPlacementManagement.Models.Entities;

namespace CampusPlacementManagement.Interfaces.Repositories;

public interface ICompanyHRRepository
{
    Task<CompanyHR?> GetByIdAsync(int id);

    Task<CompanyHR?> GetByUserIdAsync(string userId);

    Task<IEnumerable<CompanyHR>> GetAllAsync();

    Task<IEnumerable<CompanyHR>> GetPendingAsync();

    Task AddAsync(CompanyHR companyHR);

    Task UpdateAsync(CompanyHR companyHR);
}