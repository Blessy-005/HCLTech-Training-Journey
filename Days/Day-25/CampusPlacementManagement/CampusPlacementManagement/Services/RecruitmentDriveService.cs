using CampusPlacementManagement.DTOs.CompanyHRs;
using CampusPlacementManagement.DTOs.PlacementOfficers;
using CampusPlacementManagement.DTOs.RecruitmentDrives;
using CampusPlacementManagement.DTOs.Students;
using CampusPlacementManagement.Interfaces.Repositories;
using CampusPlacementManagement.Interfaces.Services;
using CampusPlacementManagement.Models.Entities;
using CampusPlacementManagement.Models.Enums;

namespace CampusPlacementManagement.Services;

public class RecruitmentDriveService : IRecruitmentDriveService
{
    private readonly IRecruitmentDriveRepository _repository;
    private readonly ICompanyHRService _companyHRService;
    private readonly IPlacementOfficerService _placementOfficerService;
    private readonly IStudentService _studentService;

    public RecruitmentDriveService(
        IRecruitmentDriveRepository repository,
        ICompanyHRService companyHRService,
        IPlacementOfficerService placementOfficerService,
        IStudentService studentService)
    {
        _repository = repository;
        _companyHRService = companyHRService;
        _placementOfficerService = placementOfficerService;
        _studentService = studentService;
    }

    public async Task<RecruitmentDriveResponseDto> CreateAsync(
        CreateRecruitmentDriveDto dto, string userId)
    {
        if (dto.JobId <= 0 || dto.CollegeId <= 0)
            throw new ArgumentException("Valid Job ID and College ID are required.");

        if (dto.DriveDate <= DateTime.UtcNow)
            throw new ArgumentException("Drive date must be in the future.");

        if (dto.Venue?.Length > 255)
            throw new ArgumentException("Venue cannot exceed 255 characters.");

        var hr = await _companyHRService.GetCurrentCompanyHRAsync(userId);

        if (hr == null)
            throw new UnauthorizedAccessException("Approved Company HR account required.");

        var job = await _repository.GetJobByIdAsync(dto.JobId);

        if (job == null)
            throw new ArgumentException("Job opening not found.");

        if (job.CompanyId != hr.CompanyId)
            throw new UnauthorizedAccessException("You can create drives only for your own company.");

        if (job.Status != JobStatus.Published)
            throw new InvalidOperationException("A drive can only be created for a published job.");

        if (!await _repository.IsCollegeApprovedAsync(dto.CollegeId))
            throw new ArgumentException("The selected college is not approved or does not exist.");

        if (!job.IsOpenToAllColleges &&
            !job.JobTargetColleges.Any(c => c.CollegeId == dto.CollegeId))
        {
            throw new InvalidOperationException(
                "The selected college is not targeted by this job.");
        }

        var drive = new RecruitmentDrive
        {
            JobId = dto.JobId,
            CollegeId = dto.CollegeId,
            DriveDate = dto.DriveDate,
            Venue = dto.Venue?.Trim(),
            Description = dto.Description?.Trim(),
            Status = RecruitmentDriveStatus.Scheduled
        };

        await _repository.AddAsync(drive);

        var savedDrive = await _repository.GetByIdAsync(drive.DriveId);

        return Map(savedDrive!);
    }

    public async Task<IEnumerable<RecruitmentDriveResponseDto>> GetVisibleAsync(
        string userId, string role)
    {
        IEnumerable<RecruitmentDrive> drives;

        switch (role)
        {
            case "SuperAdmin":
                drives = await _repository.GetAllAsync();
                break;

            case "CompanyHR":
                var hr = await _companyHRService.GetCurrentCompanyHRAsync(userId);

                if (hr == null)
                    throw new UnauthorizedAccessException("Approved Company HR account required.");

                drives = await _repository.GetByCompanyIdAsync(hr.CompanyId);
                break;

            case "PlacementOfficer":
                var officerObject =
                    await _placementOfficerService.GetCurrentOfficerAsync(userId);

                if (officerObject is not PlacementOfficerResponseDto officer)
                    throw new UnauthorizedAccessException("Approved Placement Officer account required.");

                drives = await _repository.GetByCollegeIdAsync(officer.CollegeId);
                break;

            case "Student":
                StudentResponseDto student =
                    await _studentService.GetCurrentStudentAsync(userId);

                drives = await _repository.GetByCollegeIdAsync(student.CollegeId);
                break;

            default:
                throw new UnauthorizedAccessException("You are not authorized to view recruitment drives.");
        }

        return drives.Select(Map);
    }

    public async Task<RecruitmentDriveResponseDto?> GetByIdAsync(
        int driveId, string userId, string role)
    {
        if (driveId <= 0)
            throw new ArgumentException("Invalid recruitment drive ID.");

        var drive = await _repository.GetByIdAsync(driveId);

        if (drive == null)
            return null;

        switch (role)
        {
            case "SuperAdmin":
                break;

            case "CompanyHR":
                var hr = await _companyHRService.GetCurrentCompanyHRAsync(userId);

                if (hr == null || drive.Job.CompanyId != hr.CompanyId)
                    throw new UnauthorizedAccessException("You cannot access this recruitment drive.");
                break;

            case "PlacementOfficer":
                var officerObject =
                    await _placementOfficerService.GetCurrentOfficerAsync(userId);

                if (officerObject is not PlacementOfficerResponseDto officer ||
                    drive.CollegeId != officer.CollegeId)
                    throw new UnauthorizedAccessException("You cannot access this recruitment drive.");
                break;

            case "Student":
                var student = await _studentService.GetCurrentStudentAsync(userId);

                if (drive.CollegeId != student.CollegeId)
                    throw new UnauthorizedAccessException("You cannot access this recruitment drive.");
                break;

            default:
                throw new UnauthorizedAccessException("You are not authorized to view this recruitment drive.");
        }

        return Map(drive);
    }

    public async Task<RecruitmentDriveResponseDto?> UpdateAsync(
        int driveId, UpdateRecruitmentDriveDto dto, string userId)
    {
        if (driveId <= 0)
            throw new ArgumentException("Invalid recruitment drive ID.");

        if (dto.DriveDate <= DateTime.UtcNow)
            throw new ArgumentException("Drive date must be in the future.");

        if (dto.Venue?.Length > 255)
            throw new ArgumentException("Venue cannot exceed 255 characters.");

        var hr = await _companyHRService.GetCurrentCompanyHRAsync(userId);

        if (hr == null)
            throw new UnauthorizedAccessException("Approved Company HR account required.");

        var drive = await _repository.GetByIdAsync(driveId);

        if (drive == null)
            return null;

        if (drive.Job.CompanyId != hr.CompanyId)
            throw new UnauthorizedAccessException("You can update only your company's drives.");

        if (drive.Status != RecruitmentDriveStatus.Scheduled)
            throw new InvalidOperationException("Only scheduled drives can be updated.");

        drive.DriveDate = dto.DriveDate;
        drive.Venue = dto.Venue?.Trim();
        drive.Description = dto.Description?.Trim();

        await _repository.UpdateAsync(drive);

        return Map(drive);
    }

    public async Task<bool> CancelAsync(int driveId, string userId)
    {
        if (driveId <= 0)
            throw new ArgumentException("Invalid recruitment drive ID.");

        var hr = await _companyHRService.GetCurrentCompanyHRAsync(userId);

        if (hr == null)
            throw new UnauthorizedAccessException("Approved Company HR account required.");

        var drive = await _repository.GetByIdAsync(driveId);

        if (drive == null)
            return false;

        if (drive.Job.CompanyId != hr.CompanyId)
            throw new UnauthorizedAccessException("You can cancel only your company's drives.");

        if (drive.Status != RecruitmentDriveStatus.Scheduled &&
            drive.Status != RecruitmentDriveStatus.InProgress)
        {
            throw new InvalidOperationException("This recruitment drive cannot be cancelled.");
        }

        drive.Status = RecruitmentDriveStatus.Cancelled;

        await _repository.UpdateAsync(drive);

        return true;
    }

    private static RecruitmentDriveResponseDto Map(RecruitmentDrive drive)
    {
        return new RecruitmentDriveResponseDto
        {
            DriveId = drive.DriveId,
            JobId = drive.JobId,
            JobTitle = drive.Job.JobTitle,
            CompanyId = drive.Job.CompanyId,
            CompanyName = drive.Job.Company.CompanyName,
            CollegeId = drive.CollegeId,
            CollegeName = drive.College.CollegeName,
            DriveDate = drive.DriveDate,
            Venue = drive.Venue,
            Description = drive.Description,
            Status = drive.Status
        };
    }
}
