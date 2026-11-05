using CampusPlacementManagement.DTOs.Colleges;
using CampusPlacementManagement.Models.Entities;

namespace CampusPlacementManagement.Interfaces.Services;

public interface ICollegeService
{
    Task<College?> GetByIdAsync(int collegeId);

    Task<List<College>> GetAllAsync();

    Task<College> CreateAsync(CreateCollegeDto dto);

    Task<bool> UpdateAsync(int collegeId, UpdateCollegeDto dto);

    Task<bool> UpdateStatusAsync(int collegeId, UpdateCollegeStatusDto dto);
}
