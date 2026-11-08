using CampusPlacementManagement.Models.Entities;

namespace CampusPlacementManagement.Interfaces.Repositories;

public interface IJobOpeningRepository
{
    Task<JobOpening?> GetByIdAsync(int jobId);
    Task<IEnumerable<JobOpening>> GetByCompanyIdAsync(int companyId);
    Task AddAsync(JobOpening jobOpening);
    Task UpdateAsync(JobOpening jobOpening);
    Task<bool> CollegeExistsAsync(int collegeId);
    Task<bool> IsCollegeApprovedAsync(int collegeId);
    Task RemoveTargetCollegeAsync(JobTargetCollege targetCollege);
}
