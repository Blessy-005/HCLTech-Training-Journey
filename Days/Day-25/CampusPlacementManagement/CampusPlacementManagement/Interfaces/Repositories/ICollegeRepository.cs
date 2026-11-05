using CampusPlacementManagement.Models.Entities;

namespace CampusPlacementManagement.Interfaces.Repositories;

public interface ICollegeRepository
{
    Task<College?> GetByIdAsync(int collegeId);

    Task<List<College>> GetAllAsync();

    Task<College?> GetByCodeAsync(string collegeCode);

    Task AddAsync(College college);

    Task UpdateAsync(College college);
}
