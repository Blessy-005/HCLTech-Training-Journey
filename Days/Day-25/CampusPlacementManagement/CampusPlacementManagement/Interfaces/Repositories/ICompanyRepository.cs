using CampusPlacementManagement.Models.Entities;

namespace CampusPlacementManagement.Interfaces.Repositories;

public interface ICompanyRepository
{
    Task<Company?> GetByIdAsync(int id);
    Task<IEnumerable<Company>> GetAllAsync();
    Task<IEnumerable<Company>> GetPendingAsync();
    Task<bool> ExistsByEmailAsync(string companyEmail);
    Task<bool> IsCompanyHRForCompanyAsync(string userId, int companyId);
    Task AddAsync(Company company);
    Task UpdateAsync(Company company);
}



