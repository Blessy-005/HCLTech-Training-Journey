using CampusPlacementManagement.DTOs.Reports;

namespace CampusPlacementManagement.Interfaces.Services;

public interface IReportsService
{
    Task<PlatformOverviewDto> GetPlatformOverviewAsync();

    Task<CollegeOverviewDto> GetCollegeOverviewAsync(
        string placementOfficerUserId);

    Task<CompanyRecruitmentDto> GetCompanyRecruitmentAsync(
        string companyHRUserId);

    Task<StudentPlacementDto> GetStudentPlacementAsync(
        string placementOfficerUserId);
}
