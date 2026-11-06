using CampusPlacementManagement.Models.Entities;

namespace CampusPlacementManagement.Interfaces.Repositories;

public interface IRecruitmentDriveRepository
{
    Task<RecruitmentDrive?> GetByIdAsync(int driveId);
    Task<IEnumerable<RecruitmentDrive>> GetAllAsync();
    Task<IEnumerable<RecruitmentDrive>> GetByCompanyIdAsync(int companyId);
    Task<IEnumerable<RecruitmentDrive>> GetByCollegeIdAsync(int collegeId);
    Task<JobOpening?> GetJobByIdAsync(int jobId);
    Task<bool> IsCollegeApprovedAsync(int collegeId);
    Task AddAsync(RecruitmentDrive drive);
    Task UpdateAsync(RecruitmentDrive drive);
}
