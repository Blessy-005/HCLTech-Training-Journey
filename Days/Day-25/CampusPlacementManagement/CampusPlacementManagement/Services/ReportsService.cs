using CampusPlacementManagement.Data;
using CampusPlacementManagement.DTOs.Reports;
using CampusPlacementManagement.Interfaces.Repositories;
using CampusPlacementManagement.Interfaces.Services;
using CampusPlacementManagement.Models.Entities;
using CampusPlacementManagement.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagement.Services;

public class ReportsService : IReportsService
{
    private readonly ApplicationDbContext _context;
    private readonly IPlacementOfficerRepository _placementOfficerRepository;
    private readonly ICompanyHRRepository _companyHRRepository;

    public ReportsService(
        ApplicationDbContext context,
        IPlacementOfficerRepository placementOfficerRepository,
        ICompanyHRRepository companyHRRepository)
    {
        _context = context;
        _placementOfficerRepository = placementOfficerRepository;
        _companyHRRepository = companyHRRepository;
    }

    public async Task<PlatformOverviewDto> GetPlatformOverviewAsync()
    {
        return new PlatformOverviewDto
        {
            TotalColleges = await _context.Colleges
                .AsNoTracking()
                .CountAsync(),

            ApprovedColleges = await _context.Colleges
                .AsNoTracking()
                .CountAsync(c => c.Status == CollegeStatus.Approved),

            TotalCompanies = await _context.Companies
                .AsNoTracking()
                .CountAsync(),

            ApprovedCompanies = await _context.Companies
                .AsNoTracking()
                .CountAsync(c => c.Status == CompanyStatus.Approved),

            TotalStudents = await _context.Students
                .AsNoTracking()
                .CountAsync(),

            ApprovedStudents = await _context.Students
                .AsNoTracking()
                .CountAsync(s => s.Status == StudentStatus.Approved),

            TotalJobOpenings = await _context.JobOpenings
                .AsNoTracking()
                .CountAsync(),

            PublishedJobOpenings = await _context.JobOpenings
                .AsNoTracking()
                .CountAsync(j => j.Status == JobStatus.Published),

            TotalApplications = await _context.JobApplications
                .AsNoTracking()
                .CountAsync(),

            SelectedApplications = await _context.JobApplications
                .AsNoTracking()
                .CountAsync(a => a.Status == JobApplicationStatus.Selected),

            PlacedStudents = await _context.JobApplications
                .AsNoTracking()
                .Where(a => a.Status == JobApplicationStatus.Selected)
                .Select(a => a.StudentId)
                .Distinct()
                .CountAsync()
        };
    }

    public async Task<CollegeOverviewDto> GetCollegeOverviewAsync(
        string placementOfficerUserId)
    {
        var officer = await GetApprovedOfficerAsync(placementOfficerUserId);
        var collegeId = officer.CollegeId;

        var college = officer.College;

        var collegeApplications = _context.JobApplications
            .AsNoTracking()
            .Where(a => a.Student.CollegeId == collegeId);

        var availableJobs = _context.JobOpenings
            .AsNoTracking()
            .Where(j =>
                j.Status == JobStatus.Published &&
                (j.IsOpenToAllColleges ||
                 j.JobTargetColleges.Any(t => t.CollegeId == collegeId)));

        return new CollegeOverviewDto
        {
            CollegeId = college.CollegeId,
            CollegeName = college.CollegeName,
            CollegeCode = college.CollegeCode,

            TotalStudents = await _context.Students
                .AsNoTracking()
                .CountAsync(s => s.CollegeId == collegeId),

            ApprovedStudents = await _context.Students
                .AsNoTracking()
                .CountAsync(s =>
                    s.CollegeId == collegeId &&
                    s.Status == StudentStatus.Approved),

            TotalApplications = await collegeApplications.CountAsync(),

            SelectedApplications = await collegeApplications
                .CountAsync(a => a.Status == JobApplicationStatus.Selected),

            PlacedStudents = await collegeApplications
                .Where(a => a.Status == JobApplicationStatus.Selected)
                .Select(a => a.StudentId)
                .Distinct()
                .CountAsync(),

            AvailableJobs = await availableJobs.CountAsync(),

            TotalRecruitmentDrives = await _context.RecruitmentDrives
                .AsNoTracking()
                .CountAsync(d => d.CollegeId == collegeId)
        };
    }

    public async Task<CompanyRecruitmentDto> GetCompanyRecruitmentAsync(
        string companyHRUserId)
    {
        var companyHR = await GetApprovedCompanyHRAsync(companyHRUserId);
        var companyId = companyHR.CompanyId;

        var companyApplications = _context.JobApplications
            .AsNoTracking()
            .Where(a => a.Job.CompanyId == companyId);

        return new CompanyRecruitmentDto
        {
            CompanyId = companyHR.Company.CompanyId,
            CompanyName = companyHR.Company.CompanyName,

            TotalJobOpenings = await _context.JobOpenings
                .AsNoTracking()
                .CountAsync(j => j.CompanyId == companyId),

            PublishedJobOpenings = await _context.JobOpenings
                .AsNoTracking()
                .CountAsync(j =>
                    j.CompanyId == companyId &&
                    j.Status == JobStatus.Published),

            TotalApplications = await companyApplications.CountAsync(),

            SelectedApplications = await companyApplications
                .CountAsync(a => a.Status == JobApplicationStatus.Selected),

            PlacedStudents = await companyApplications
                .Where(a => a.Status == JobApplicationStatus.Selected)
                .Select(a => a.StudentId)
                .Distinct()
                .CountAsync(),

            TotalInterviewSchedules = await _context.InterviewSchedules
                .AsNoTracking()
                .CountAsync(i => i.Application.Job.CompanyId == companyId)
        };
    }

    public async Task<StudentPlacementDto> GetStudentPlacementAsync(
        string placementOfficerUserId)
    {
        var officer = await GetApprovedOfficerAsync(placementOfficerUserId);
        var collegeId = officer.CollegeId;

        var students = await _context.Students
            .AsNoTracking()
            .Include(s => s.User)
            .Where(s => s.CollegeId == collegeId)
            .OrderBy(s => s.RegisterNumber)
            .ToListAsync();

        var studentIds = students
            .Select(s => s.StudentId)
            .ToList();

        var applicationStats = await _context.JobApplications
            .AsNoTracking()
            .Where(a => studentIds.Contains(a.StudentId))
            .GroupBy(a => a.StudentId)
            .Select(g => new
            {
                StudentId = g.Key,
                TotalApplications = g.Count(),
                SelectedApplications = g.Count(
                    a => a.Status == JobApplicationStatus.Selected),
                LastAppliedAt = g.Max(a => (DateTime?)a.AppliedAt)
            })
            .ToDictionaryAsync(x => x.StudentId);

        var studentItems = students.Select(s =>
        {
            applicationStats.TryGetValue(s.StudentId, out var stats);

            return new StudentPlacementItemDto
            {
                StudentId = s.StudentId,
                FullName = s.User.FullName,
                RegisterNumber = s.RegisterNumber,
                Department = s.Department,
                GraduationYear = s.GraduationYear,
                CGPA = s.CGPA,
                Status = s.Status.ToString(),

                TotalApplications = stats?.TotalApplications ?? 0,
                SelectedApplications = stats?.SelectedApplications ?? 0,
                LastAppliedAt = stats?.LastAppliedAt
            };
        }).ToList();

        var collegeApplications = _context.JobApplications
            .AsNoTracking()
            .Where(a => a.Student.CollegeId == collegeId);

        return new StudentPlacementDto
        {
            CollegeId = officer.College.CollegeId,
            CollegeName = officer.College.CollegeName,

            TotalStudents = students.Count,
            ApprovedStudents = students.Count(
                s => s.Status == StudentStatus.Approved),

            TotalApplications = await collegeApplications.CountAsync(),

            SelectedApplications = await collegeApplications
                .CountAsync(a => a.Status == JobApplicationStatus.Selected),

            PlacedStudents = await collegeApplications
                .Where(a => a.Status == JobApplicationStatus.Selected)
                .Select(a => a.StudentId)
                .Distinct()
                .CountAsync(),

            Students = studentItems
        };
    }

    private async Task<PlacementOfficer> GetApprovedOfficerAsync(
        string userId)
    {
        var officer = await _placementOfficerRepository
            .GetByUserIdAsync(userId);

        if (officer == null ||
            officer.Status != PlacementOfficerStatus.Approved ||
            officer.User.AccountStatus != AccountStatus.Approved ||
            officer.College.Status != CollegeStatus.Approved)
        {
            throw new UnauthorizedAccessException(
                "An approved placement officer account and college are required.");
        }

        return officer;
    }

    private async Task<CompanyHR> GetApprovedCompanyHRAsync(
        string userId)
    {
        var companyHR = await _companyHRRepository
            .GetByUserIdAsync(userId);

        if (companyHR == null ||
            companyHR.Status != CompanyHRStatus.Approved ||
            companyHR.User.AccountStatus != AccountStatus.Approved ||
            companyHR.Company.Status != CompanyStatus.Approved)
        {
            throw new UnauthorizedAccessException(
                "An approved Company HR account and company are required.");
        }

        return companyHR;
    }
}
