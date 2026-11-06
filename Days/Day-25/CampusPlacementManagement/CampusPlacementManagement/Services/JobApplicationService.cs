using CampusPlacementManagement.Data;
using CampusPlacementManagement.DTOs.Applications;
using CampusPlacementManagement.DTOs.CompanyHRs;
using CampusPlacementManagement.Interfaces.Repositories;
using CampusPlacementManagement.Interfaces.Services;
using CampusPlacementManagement.Models.Entities;
using CampusPlacementManagement.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagement.Services;

public class JobApplicationService : IJobApplicationService
{
    private readonly ApplicationDbContext _context;
    private readonly IStudentRepository _studentRepository;
    private readonly IJobOpeningRepository _jobOpeningRepository;
    private readonly ICompanyHRService _companyHRService;
    private readonly IApplicationStatusHistoryService _historyService;

    public JobApplicationService(
        ApplicationDbContext context,
        IStudentRepository studentRepository,
        IJobOpeningRepository jobOpeningRepository,
        ICompanyHRService companyHRService,
        IApplicationStatusHistoryService historyService)
    {
        _context = context;
        _studentRepository = studentRepository;
        _jobOpeningRepository = jobOpeningRepository;
        _companyHRService = companyHRService;
        _historyService = historyService;
    }

    public async Task<JobApplicationResponseDto> ApplyAsync(
        ApplyJobDto dto, string userId)
    {
        var student = await _studentRepository.GetByUserIdAsync(userId);

        if (student == null)
            throw new KeyNotFoundException("Student profile not found.");

        if (student.Status != StudentStatus.Approved ||
            student.User.AccountStatus != AccountStatus.Approved ||
            student.College.Status != CollegeStatus.Approved)
        {
            throw new UnauthorizedAccessException(
                "Your student account and college must be approved before applying.");
        }

        var job = await _jobOpeningRepository.GetByIdAsync(dto.JobId);

        if (job == null)
            throw new KeyNotFoundException("Job opening not found.");

        if (job.Status != JobStatus.Published)
            throw new ArgumentException("This job is not currently published.");

        if (job.ApplicationDeadline <= DateTime.UtcNow)
            throw new ArgumentException("The application deadline has passed.");

        var collegeEligible = job.IsOpenToAllColleges ||
            job.JobTargetColleges.Any(t => t.CollegeId == student.CollegeId);

        if (!collegeEligible)
            throw new ArgumentException(
                "This job is not available to students from your college.");

        if (job.MinimumCGPA.HasValue &&
            student.CGPA < job.MinimumCGPA.Value)
        {
            throw new ArgumentException(
                $"Your CGPA does not meet the minimum requirement of {job.MinimumCGPA.Value}.");
        }

        if (job.MaximumBacklogs.HasValue &&
            student.ActiveBacklogs > job.MaximumBacklogs.Value)
        {
            throw new ArgumentException(
                "Your active backlog count exceeds the job's allowed limit.");
        }

        if (job.EligibleDepartments.Any() &&
            !job.EligibleDepartments.Any(d =>
                string.Equals(d.Department, student.Department,
                    StringComparison.OrdinalIgnoreCase)))
        {
            throw new ArgumentException(
                "Your department is not eligible for this job.");
        }

        if (job.EligibleGraduationYears.Any() &&
            !job.EligibleGraduationYears.Any(y =>
                y.GraduationYear == student.GraduationYear))
        {
            throw new ArgumentException(
                "Your graduation year is not eligible for this job.");
        }

        var alreadyApplied = await _context.JobApplications.AnyAsync(a =>
            a.JobId == dto.JobId && a.StudentId == student.StudentId);

        if (alreadyApplied)
            throw new ArgumentException(
                "You have already applied for this job.");

        var now = DateTime.UtcNow;

        var application = new JobApplication
        {
            JobId = job.JobId,
            StudentId = student.StudentId,
            AppliedAt = now,
            LastUpdatedAt = now,
            Status = JobApplicationStatus.Applied,
            ResumeUrl = dto.ResumeUrl ?? student.ResumeUrl
        };

        var history = new ApplicationStatusHistory
        {
            Application = application,
            PreviousStatus = null,
            NewStatus = JobApplicationStatus.Applied,
            UpdatedByUserId = userId,
            Remarks = "Application submitted.",
            UpdatedAt = now
        };

        _context.JobApplications.Add(application);
        _context.ApplicationStatusHistories.Add(history);

        await _context.SaveChangesAsync();

        return Map(application);
    }

    public async Task<IEnumerable<JobApplicationResponseDto>>
        GetMyApplicationsAsync(string userId)
    {
        var student = await _studentRepository.GetByUserIdAsync(userId);

        if (student == null)
            throw new KeyNotFoundException("Student profile not found.");

        if (student.Status != StudentStatus.Approved ||
            student.User.AccountStatus != AccountStatus.Approved ||
            student.College.Status != CollegeStatus.Approved)
        {
            throw new UnauthorizedAccessException(
                "Your student account is not currently approved.");
        }

        var applications = await _context.JobApplications
            .AsNoTracking()
            .Include(a => a.Job).ThenInclude(j => j.Company)
            .Include(a => a.Student).ThenInclude(s => s.User)
            .Include(a => a.Student).ThenInclude(s => s.College)
            .Where(a => a.StudentId == student.StudentId)
            .OrderByDescending(a => a.AppliedAt)
            .ToListAsync();

        return applications.Select(Map);
    }

    public async Task<JobApplicationResponseDto> GetByIdAsync(
        int applicationId, string userId, string role)
    {
        var application = await GetApplicationEntityAsync(applicationId);

        if (application == null)
            throw new KeyNotFoundException("Job application not found.");

        if (!await CanAccessApplicationAsync(application, userId, role))
            throw new UnauthorizedAccessException(
                "You are not authorized to view this application.");

        return Map(application);
    }

    public async Task<IEnumerable<JobApplicationResponseDto>> GetForJobAsync(
        int jobId, string userId, string role)
    {
        var job = await _jobOpeningRepository.GetByIdAsync(jobId);

        if (job == null)
            throw new KeyNotFoundException("Job opening not found.");

        if (role == "CompanyHR")
        {
            var hr = await _companyHRService.GetCurrentCompanyHRAsync(userId);

            if (hr == null || hr.CompanyId != job.CompanyId)
                throw new UnauthorizedAccessException(
                    "You can only view applicants for your own company's jobs.");
        }
        else if (role == "PlacementOfficer")
        {
            var collegeId = await GetApprovedOfficerCollegeIdAsync(userId);

            if (!job.IsOpenToAllColleges &&
                !job.JobTargetColleges.Any(t => t.CollegeId == collegeId))
            {
                // The officer can still view applicants from their college
                // if the job was targeted to that college.
                throw new UnauthorizedAccessException(
                    "This job is not associated with your college.");
            }
        }
        else if (role != "SuperAdmin")
        {
            throw new UnauthorizedAccessException(
                "You are not authorized to view job applicants.");
        }

        var query = _context.JobApplications
            .AsNoTracking()
            .Include(a => a.Job).ThenInclude(j => j.Company)
            .Include(a => a.Student).ThenInclude(s => s.User)
            .Include(a => a.Student).ThenInclude(s => s.College)
            .Where(a => a.JobId == jobId);

        if (role == "PlacementOfficer")
        {
            var collegeId = await GetApprovedOfficerCollegeIdAsync(userId);
            query = query.Where(a => a.Student.CollegeId == collegeId);
        }

        var applications = await query
            .OrderByDescending(a => a.AppliedAt)
            .ToListAsync();

        return applications.Select(Map);
    }

    public async Task WithdrawAsync(int applicationId, string userId)
    {
        var dto = new UpdateApplicationStatusDto
        {
            NewStatus = JobApplicationStatus.Withdrawn,
            Remarks = "Application withdrawn by student."
        };

        await _historyService.UpdateStatusAsync(
            applicationId, dto, userId);
    }

    private async Task<JobApplication?> GetApplicationEntityAsync(
        int applicationId)
    {
        return await _context.JobApplications
            .AsNoTracking()
            .Include(a => a.Job).ThenInclude(j => j.Company)
            .Include(a => a.Student).ThenInclude(s => s.User)
            .Include(a => a.Student).ThenInclude(s => s.College)
            .FirstOrDefaultAsync(a => a.ApplicationId == applicationId);
    }

    private async Task<bool> CanAccessApplicationAsync(
        JobApplication application, string userId, string role)
    {
        if (role == "SuperAdmin")
            return true;

        if (role == "Student")
        {
            return await _context.Students.AnyAsync(s =>
                s.UserId == userId &&
                s.StudentId == application.StudentId &&
                s.Status == StudentStatus.Approved &&
                s.User.AccountStatus == AccountStatus.Approved &&
                s.College.Status == CollegeStatus.Approved);
        }

        if (role == "CompanyHR")
        {
            var hr = await _companyHRService.GetCurrentCompanyHRAsync(userId);
            return hr != null && hr.CompanyId == application.Job.CompanyId;
        }

        if (role == "PlacementOfficer")
        {
            var collegeId = await GetApprovedOfficerCollegeIdAsync(userId);
            return application.Student.CollegeId == collegeId;
        }

        return false;
    }

    private async Task<int> GetApprovedOfficerCollegeIdAsync(string userId)
    {
        var officer = await _context.PlacementOfficers
            .Include(p => p.User)
            .Include(p => p.College)
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (officer == null ||
            officer.Status != PlacementOfficerStatus.Approved ||
            officer.User.AccountStatus != AccountStatus.Approved ||
            officer.College.Status != CollegeStatus.Approved)
        {
            throw new UnauthorizedAccessException(
                "Your placement officer account or college is not approved.");
        }

        return officer.CollegeId;
    }

    private static JobApplicationResponseDto Map(JobApplication application)
    {
        return new JobApplicationResponseDto
        {
            ApplicationId = application.ApplicationId,
            JobId = application.JobId,
            JobTitle = application.Job.JobTitle,
            CompanyName = application.Job.Company.CompanyName,
            StudentId = application.StudentId,
            StudentName = application.Student.User.FullName,
            StudentEmail = application.Student.User.Email ?? string.Empty,
            CollegeId = application.Student.CollegeId,
            CollegeName = application.Student.College.CollegeName,
            Department = application.Student.Department,
            GraduationYear = application.Student.GraduationYear,
            CGPA = application.Student.CGPA,
            ResumeUrl = application.ResumeUrl,
            AppliedAt = application.AppliedAt,
            Status = application.Status,
            LastUpdatedAt = application.LastUpdatedAt
        };
    }
}
