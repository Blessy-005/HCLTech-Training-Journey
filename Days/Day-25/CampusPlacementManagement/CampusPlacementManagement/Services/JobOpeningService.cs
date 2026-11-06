using CampusPlacementManagement.DTOs.Jobs;
using CampusPlacementManagement.Interfaces.Repositories;
using CampusPlacementManagement.Interfaces.Services;
using CampusPlacementManagement.Models.Entities;
using CampusPlacementManagement.Models.Enums;

namespace CampusPlacementManagement.Services;

public class JobOpeningService : IJobOpeningService
{
    private readonly IJobOpeningRepository _jobRepository;
    private readonly ICompanyHRService _companyHRService;

    public JobOpeningService(
        IJobOpeningRepository jobRepository,
        ICompanyHRService companyHRService)
    {
        _jobRepository = jobRepository;
        _companyHRService = companyHRService;
    }

    public async Task<JobOpeningResponseDto> CreateAsync(
        CreateJobOpeningDto dto,
        string userId)
    {
        var hr = await GetApprovedHRAsync(userId);

        ValidateEligibility(dto.EligibleDepartments, dto.EligibleGraduationYears);

        var job = new JobOpening
        {
            CompanyId = hr.CompanyId,
            CreatedByHRId = hr.CompanyHRId,
            JobTitle = dto.JobTitle.Trim(),
            JobDescription = dto.JobDescription.Trim(),
            JobType = dto.JobType.Trim(),
            Location = dto.Location.Trim(),
            SalaryPackage = dto.SalaryPackage,
            MinimumCGPA = dto.MinimumCGPA,
            MaximumBacklogs = dto.MaximumBacklogs,
            ApplicationDeadline = dto.ApplicationDeadline,
            IsOpenToAllColleges = dto.IsOpenToAllColleges,
            Status = JobStatus.Draft,
            CreatedAt = DateTime.UtcNow,
            EligibleDepartments = dto.EligibleDepartments
                .Select(d => d.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Select(d => new JobEligibleDepartment { Department = d })
                .ToList(),
            EligibleGraduationYears = dto.EligibleGraduationYears
                .Distinct()
                .Select(y => new JobEligibleGraduationYear { GraduationYear = y })
                .ToList()
        };

        await _jobRepository.AddAsync(job);

        var savedJob = await _jobRepository.GetByIdAsync(job.JobId);

        return MapToResponse(savedJob
            ?? throw new InvalidOperationException("Unable to retrieve the created job."));
    }

    public async Task<IEnumerable<JobOpeningResponseDto>> GetMyJobsAsync(
        string userId)
    {
        var hr = await GetApprovedHRAsync(userId);
        var jobs = await _jobRepository.GetByCompanyIdAsync(hr.CompanyId);

        return jobs.Select(MapToResponse);
    }

    public async Task<JobOpeningResponseDto?> GetByIdAsync(
        int jobId,
        string userId)
    {
        var hr = await GetApprovedHRAsync(userId);
        var job = await _jobRepository.GetByIdAsync(jobId);

        if (job == null || job.CompanyId != hr.CompanyId)
            return null;

        return MapToResponse(job);
    }

    public async Task<JobOpeningResponseDto?> UpdateAsync(
        int jobId,
        UpdateJobOpeningDto dto,
        string userId)
    {
        var hr = await GetApprovedHRAsync(userId);
        var job = await _jobRepository.GetByIdAsync(jobId);

        if (job == null || job.CompanyId != hr.CompanyId)
            return null;

        if (job.Status != JobStatus.Draft)
            throw new InvalidOperationException(
                "Only draft job openings can be updated.");

        ValidateEligibility(dto.EligibleDepartments, dto.EligibleGraduationYears);

        job.JobTitle = dto.JobTitle.Trim();
        job.JobDescription = dto.JobDescription.Trim();
        job.JobType = dto.JobType.Trim();
        job.Location = dto.Location.Trim();
        job.SalaryPackage = dto.SalaryPackage;
        job.MinimumCGPA = dto.MinimumCGPA;
        job.MaximumBacklogs = dto.MaximumBacklogs;
        job.ApplicationDeadline = dto.ApplicationDeadline;
        job.IsOpenToAllColleges = dto.IsOpenToAllColleges;

        job.EligibleDepartments.Clear();
        foreach (var department in dto.EligibleDepartments
            .Select(d => d.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase))
        {
            job.EligibleDepartments.Add(
                new JobEligibleDepartment { Department = department });
        }

        job.EligibleGraduationYears.Clear();
        foreach (var year in dto.EligibleGraduationYears.Distinct())
        {
            job.EligibleGraduationYears.Add(
                new JobEligibleGraduationYear { GraduationYear = year });
        }

        if (job.IsOpenToAllColleges)
            job.JobTargetColleges.Clear();

        await _jobRepository.UpdateAsync(job);

        return MapToResponse(job);
    }

    public async Task<bool> PublishAsync(int jobId, string userId)
    {
        var hr = await GetApprovedHRAsync(userId);
        var job = await _jobRepository.GetByIdAsync(jobId);

        if (job == null || job.CompanyId != hr.CompanyId)
            return false;

        if (job.Status != JobStatus.Draft)
            throw new InvalidOperationException(
                "Only draft job openings can be published.");

        if (job.ApplicationDeadline <= DateTime.UtcNow)
            throw new InvalidOperationException(
                "The application deadline must be in the future.");

        if (!job.IsOpenToAllColleges && !job.JobTargetColleges.Any())
            throw new InvalidOperationException(
                "Select at least one target college or open the job to all colleges.");

        job.Status = JobStatus.Published;

        await _jobRepository.UpdateAsync(job);
        return true;
    }

    public async Task<bool> CloseAsync(int jobId, string userId)
    {
        var hr = await GetApprovedHRAsync(userId);
        var job = await _jobRepository.GetByIdAsync(jobId);

        if (job == null || job.CompanyId != hr.CompanyId)
            return false;

        if (job.Status != JobStatus.Published)
            throw new InvalidOperationException(
                "Only published job openings can be closed.");

        job.Status = JobStatus.Closed;

        await _jobRepository.UpdateAsync(job);
        return true;
    }

    public async Task<bool> SetTargetCollegesAsync(
        int jobId,
        SetJobTargetCollegesDto dto,
        string userId)
    {
        var hr = await GetApprovedHRAsync(userId);
        var job = await _jobRepository.GetByIdAsync(jobId);

        if (job == null || job.CompanyId != hr.CompanyId)
            return false;

        if (job.Status != JobStatus.Draft)
            throw new InvalidOperationException(
                "Target colleges can only be changed for draft jobs.");

        if (job.IsOpenToAllColleges)
            throw new InvalidOperationException(
                "This job is open to all colleges. Disable that option before selecting target colleges.");

        var collegeIds = dto.CollegeIds.Distinct().ToList();

        if (collegeIds.Count == 0)
            throw new InvalidOperationException(
                "Select at least one target college.");

        foreach (var collegeId in collegeIds)
        {
            if (collegeId <= 0 ||
                !await _jobRepository.IsCollegeApprovedAsync(collegeId))
            {
                throw new InvalidOperationException(
                    $"College ID {collegeId} does not exist or is not approved.");
            }
        }

        job.JobTargetColleges.Clear();

        foreach (var collegeId in collegeIds)
        {
            job.JobTargetColleges.Add(
                new JobTargetCollege { CollegeId = collegeId });
        }

        await _jobRepository.UpdateAsync(job);
        return true;
    }

    public async Task<IEnumerable<int>?> GetTargetCollegesAsync(
        int jobId,
        string userId)
    {
        var hr = await GetApprovedHRAsync(userId);
        var job = await _jobRepository.GetByIdAsync(jobId);

        if (job == null || job.CompanyId != hr.CompanyId)
            return null;

        return job.JobTargetColleges.Select(t => t.CollegeId).ToList();
    }

    public async Task<bool> RemoveTargetCollegeAsync(
        int jobId,
        int collegeId,
        string userId)
    {
        var hr = await GetApprovedHRAsync(userId);
        var job = await _jobRepository.GetByIdAsync(jobId);

        if (job == null || job.CompanyId != hr.CompanyId)
            return false;

        if (job.Status != JobStatus.Draft)
            throw new InvalidOperationException(
                "Target colleges can only be changed for draft jobs.");

        var target = job.JobTargetColleges
            .FirstOrDefault(t => t.CollegeId == collegeId);

        if (target == null)
            return false;

        job.JobTargetColleges.Remove(target);

        await _jobRepository.UpdateAsync(job);
        return true;
    }

    private async Task<DTOs.CompanyHRs.CompanyHRResponseDto> GetApprovedHRAsync(
        string userId)
    {
        var hr = await _companyHRService.GetCurrentCompanyHRAsync(userId);

        if (hr == null)
            throw new UnauthorizedAccessException(
                "An approved Company HR account is required.");

        return hr;
    }

    private static void ValidateEligibility(
        IEnumerable<string> departments,
        IEnumerable<int> graduationYears)
    {
        if (departments.Any(d => string.IsNullOrWhiteSpace(d)))
            throw new ArgumentException(
                "Eligible department names cannot be empty.");

        if (graduationYears.Any(y => y < 2000 || y > 2100))
            throw new ArgumentException(
                "Graduation years must be between 2000 and 2100.");
    }

    private static JobOpeningResponseDto MapToResponse(JobOpening job)
    {
        return new JobOpeningResponseDto
        {
            JobId = job.JobId,
            CompanyId = job.CompanyId,
            CompanyName = job.Company?.CompanyName ?? string.Empty,
            CreatedByHRId = job.CreatedByHRId,
            JobTitle = job.JobTitle,
            JobDescription = job.JobDescription,
            JobType = job.JobType,
            Location = job.Location,
            SalaryPackage = job.SalaryPackage,
            MinimumCGPA = job.MinimumCGPA,
            MaximumBacklogs = job.MaximumBacklogs,
            ApplicationDeadline = job.ApplicationDeadline,
            IsOpenToAllColleges = job.IsOpenToAllColleges,
            Status = job.Status,
            CreatedAt = job.CreatedAt,
            EligibleDepartments = job.EligibleDepartments
                .Select(d => d.Department).ToList(),
            EligibleGraduationYears = job.EligibleGraduationYears
                .Select(y => y.GraduationYear).ToList(),
            TargetCollegeIds = job.JobTargetColleges
                .Select(t => t.CollegeId).ToList()
        };
    }
}
