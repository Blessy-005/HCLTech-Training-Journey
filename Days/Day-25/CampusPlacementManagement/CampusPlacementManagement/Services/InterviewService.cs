
using CampusPlacementManagement.Data;
using CampusPlacementManagement.DTOs.Applications;
using CampusPlacementManagement.DTOs.Interviews;
using CampusPlacementManagement.Interfaces.Services;
using CampusPlacementManagement.Models.Entities;
using CampusPlacementManagement.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagement.Services;

public class InterviewService : IInterviewService
{
    private readonly ApplicationDbContext _context;
    private readonly IApplicationStatusHistoryService _historyService;

    public InterviewService(
        ApplicationDbContext context,
        IApplicationStatusHistoryService historyService)
    {
        _context = context;
        _historyService = historyService;
    }

    public async Task<InterviewRoundResponseDto> CreateRoundAsync(
        int jobId, CreateInterviewRoundDto dto, string userId)
    {
        if (jobId <= 0 || dto.RoundNumber <= 0 ||
            string.IsNullOrWhiteSpace(dto.RoundName) ||
            dto.RoundName.Length > 100)
            throw new ArgumentException("Invalid interview round details.");

        var job = await _context.JobOpenings
            .Include(j => j.Company)
            .FirstOrDefaultAsync(j => j.JobId == jobId);

        if (job == null)
            throw new KeyNotFoundException("Job not found.");

        await EnsureApprovedHRAsync(userId, job.CompanyId);

        if (job.Status != JobStatus.Published)
            throw new InvalidOperationException(
                "Interview rounds can only be created for published jobs.");

        var duplicate = await _context.InterviewRounds.AnyAsync(r =>
            r.JobId == jobId &&
            (r.RoundNumber == dto.RoundNumber ||
             r.RoundName.ToLower() == dto.RoundName.Trim().ToLower()));

        if (duplicate)
            throw new InvalidOperationException(
                "Round number or round name already exists for this job.");

        var round = new InterviewRound
        {
            JobId = jobId,
            RoundName = dto.RoundName.Trim(),
            RoundNumber = dto.RoundNumber,
            Description = dto.Description
        };

        _context.InterviewRounds.Add(round);
        await _context.SaveChangesAsync();

        return MapRound(round, job.JobTitle);
    }

    public async Task<IEnumerable<InterviewRoundResponseDto>> GetRoundsAsync(
        int jobId, string userId, string role)
    {
        var job = await _context.JobOpenings
            .FirstOrDefaultAsync(j => j.JobId == jobId);

        if (job == null)
            throw new KeyNotFoundException("Job not found.");

        if (role == "CompanyHR")
        {
            await EnsureApprovedHRAsync(userId, job.CompanyId);
        }
        else if (role == "Student")
        {
            var student = await GetApprovedStudentAsync(userId);

            var hasApplied = await _context.JobApplications.AnyAsync(a =>
                a.JobId == jobId && a.StudentId == student.StudentId);

            if (!hasApplied)
                throw new UnauthorizedAccessException(
                    "You can only view rounds for jobs you have applied to.");
        }
        else if (role == "PlacementOfficer")
        {
            var collegeId = await GetApprovedOfficerCollegeIdAsync(userId);

            var canView = job.IsOpenToAllColleges ||
                await _context.JobTargetColleges.AnyAsync(t =>
                    t.JobId == jobId && t.CollegeId == collegeId) ||
                await _context.JobApplications.AnyAsync(a =>
                    a.JobId == jobId &&
                    a.Student.CollegeId == collegeId);

            if (!canView)
                throw new UnauthorizedAccessException(
                    "This job is not accessible to your college.");
        }
        else if (role != "SuperAdmin")
        {
            throw new UnauthorizedAccessException("Access denied.");
        }

        return await _context.InterviewRounds
            .Where(r => r.JobId == jobId)
            .OrderBy(r => r.RoundNumber)
            .Select(r => new InterviewRoundResponseDto
            {
                InterviewRoundId = r.InterviewRoundId,
                JobId = r.JobId,
                JobTitle = r.Job.JobTitle,
                RoundName = r.RoundName,
                RoundNumber = r.RoundNumber,
                Description = r.Description
            })
            .ToListAsync();
    }

    public async Task<InterviewScheduleResponseDto> ScheduleAsync(
        int applicationId, ScheduleInterviewDto dto, string userId)
    {
        if (applicationId <= 0 || dto.InterviewRoundId <= 0)
            throw new ArgumentException("Invalid application or round ID.");

        if (dto.ScheduledAt <= DateTime.UtcNow)
            throw new ArgumentException("Interview date must be in the future.");

        if (dto.MeetingLink?.Length > 500 || dto.Venue?.Length > 255)
            throw new ArgumentException("Meeting link or venue is too long.");

        var application = await GetApplicationAsync(applicationId);

        await EnsureApprovedHRAsync(userId, application.Job.CompanyId);

        if (application.Status != JobApplicationStatus.UnderReview &&
            application.Status != JobApplicationStatus.Shortlisted &&
            application.Status != JobApplicationStatus.InterviewScheduled)
        {
            throw new InvalidOperationException(
                "Only applications under review, shortlisted, or already interview-scheduled can be scheduled.");
        }

        var round = await _context.InterviewRounds
            .FirstOrDefaultAsync(r =>
                r.InterviewRoundId == dto.InterviewRoundId &&
                r.JobId == application.JobId);

        if (round == null)
            throw new ArgumentException(
                "The interview round does not belong to this job.");

        var duplicate = await _context.InterviewSchedules.AnyAsync(s =>
            s.ApplicationId == applicationId &&
            s.InterviewRoundId == dto.InterviewRoundId &&
            s.Status != InterviewScheduleStatus.Cancelled);

        if (duplicate)
            throw new InvalidOperationException(
                "An active interview is already scheduled for this round.");

        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            var schedule = new InterviewSchedule
            {
                ApplicationId = applicationId,
                InterviewRoundId = dto.InterviewRoundId,
                ScheduledAt = dto.ScheduledAt,
                MeetingLink = dto.MeetingLink,
                Venue = dto.Venue,
                Status = InterviewScheduleStatus.Scheduled
            };

            _context.InterviewSchedules.Add(schedule);
            await _context.SaveChangesAsync();

            if (application.Status != JobApplicationStatus.InterviewScheduled)
            {
                var updated = await _historyService.UpdateStatusAsync(
                    applicationId,
                    new UpdateApplicationStatusDto
                    {
                        NewStatus = JobApplicationStatus.InterviewScheduled,
                        Remarks = "Interview scheduled."
                    },
                    userId);

                if (!updated)
                    throw new InvalidOperationException(
                        "Could not update application status.");
            }

            await transaction.CommitAsync();

            return await GetScheduleResponseAsync(schedule.InterviewScheduleId);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<IEnumerable<InterviewScheduleResponseDto>> GetInterviewsAsync(
        int applicationId, string userId, string role)
    {
        var application = await GetApplicationAsync(applicationId);

        await EnsureApplicationAccessAsync(application, userId, role);

        return await _context.InterviewSchedules
            .Where(s => s.ApplicationId == applicationId)
            .OrderBy(s => s.ScheduledAt)
            .Select(s => new InterviewScheduleResponseDto
            {
                InterviewScheduleId = s.InterviewScheduleId,
                ApplicationId = s.ApplicationId,
                JobId = s.Application.JobId,
                JobTitle = s.Application.Job.JobTitle,
                StudentId = s.Application.StudentId,
                StudentName = s.Application.Student.User.FullName,
                StudentEmail = s.Application.Student.User.Email ?? "",
                InterviewRoundId = s.InterviewRoundId,
                RoundName = s.InterviewRound.RoundName,
                RoundNumber = s.InterviewRound.RoundNumber,
                ScheduledAt = s.ScheduledAt,
                MeetingLink = s.MeetingLink,
                Venue = s.Venue,
                Status = s.Status,
                Feedback = s.Feedback
            })
            .ToListAsync();
    }

    public async Task<InterviewScheduleResponseDto?> UpdateAsync(
        int interviewId, UpdateInterviewScheduleDto dto, string userId)
    {
        var schedule = await GetScheduleEntityAsync(interviewId);

        await EnsureApprovedHRAsync(userId, schedule.Application.Job.CompanyId);

        if (schedule.Status != InterviewScheduleStatus.Scheduled &&
            schedule.Status != InterviewScheduleStatus.Rescheduled)
            throw new InvalidOperationException(
                "Only scheduled or rescheduled interviews can be updated.");

        if (dto.ScheduledAt <= DateTime.UtcNow)
            throw new ArgumentException("Interview date must be in the future.");

        if (dto.MeetingLink?.Length > 500 || dto.Venue?.Length > 255)
            throw new ArgumentException("Meeting link or venue is too long.");

        if (schedule.ScheduledAt != dto.ScheduledAt)
            schedule.Status = InterviewScheduleStatus.Rescheduled;

        schedule.ScheduledAt = dto.ScheduledAt;
        schedule.MeetingLink = dto.MeetingLink;
        schedule.Venue = dto.Venue;

        await _context.SaveChangesAsync();

        return await GetScheduleResponseAsync(interviewId);
    }

    public async Task<InterviewScheduleResponseDto?> SubmitFeedbackAsync(
        int interviewId, SubmitInterviewFeedbackDto dto, string userId)
    {
        if (!Enum.IsDefined(typeof(InterviewScheduleStatus), dto.Status))
            throw new ArgumentException("Invalid interview status.");

        if (dto.Status != InterviewScheduleStatus.InProgress &&
            dto.Status != InterviewScheduleStatus.Completed &&
            dto.Status != InterviewScheduleStatus.Cancelled)
            throw new ArgumentException("Invalid feedback status.");

        if (dto.Status == InterviewScheduleStatus.Completed &&
            string.IsNullOrWhiteSpace(dto.Feedback))
            throw new ArgumentException(
                "Feedback is required when completing an interview.");

        var schedule = await GetScheduleEntityAsync(interviewId);

        await EnsureApprovedHRAsync(userId, schedule.Application.Job.CompanyId);

        if (schedule.Status == InterviewScheduleStatus.Completed ||
            schedule.Status == InterviewScheduleStatus.Cancelled)
            throw new InvalidOperationException(
                "This interview is already in a terminal status.");

        schedule.Feedback = dto.Feedback;
        schedule.Status = dto.Status;

        await _context.SaveChangesAsync();

        return await GetScheduleResponseAsync(interviewId);
    }

    private async Task EnsureApprovedHRAsync(
        string userId, int companyId)
    {
        var hr = await _context.CompanyHRs
            .Include(h => h.User)
            .Include(h => h.Company)
            .FirstOrDefaultAsync(h => h.UserId == userId);

        if (hr == null ||
            hr.Status != CompanyHRStatus.Approved ||
            hr.Company.Status != CompanyStatus.Approved ||
            hr.User.AccountStatus != AccountStatus.Approved ||
            hr.CompanyId != companyId)
        {
            throw new UnauthorizedAccessException(
                "You are not authorized to manage interviews for this company.");
        }
    }

    private async Task<Student> GetApprovedStudentAsync(string userId)
    {
        var student = await _context.Students
            .Include(s => s.User)
            .Include(s => s.College)
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (student == null ||
            student.Status != StudentStatus.Approved ||
            student.College.Status != CollegeStatus.Approved ||
            student.User.AccountStatus != AccountStatus.Approved)
            throw new UnauthorizedAccessException("Approved student access required.");

        return student;
    }

    private async Task<int> GetApprovedOfficerCollegeIdAsync(string userId)
    {
        var officer = await _context.PlacementOfficers
            .Include(p => p.User)
            .Include(p => p.College)
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (officer == null ||
            officer.Status != PlacementOfficerStatus.Approved ||
            officer.College.Status != CollegeStatus.Approved ||
            officer.User.AccountStatus != AccountStatus.Approved)
            throw new UnauthorizedAccessException(
                "Approved placement officer access required.");

        return officer.CollegeId;
    }

    private async Task<JobApplication> GetApplicationAsync(int applicationId)
    {
        var application = await _context.JobApplications
            .Include(a => a.Job)
            .Include(a => a.Student)
                .ThenInclude(s => s.User)
            .Include(a => a.Student)
                .ThenInclude(s => s.College)
            .FirstOrDefaultAsync(a => a.ApplicationId == applicationId);

        return application ?? throw new KeyNotFoundException(
            "Job application not found.");
    }

    private async Task EnsureApplicationAccessAsync(
        JobApplication application, string userId, string role)
    {
        if (role == "SuperAdmin")
            return;

        if (role == "CompanyHR")
        {
            await EnsureApprovedHRAsync(userId, application.Job.CompanyId);
            return;
        }

        if (role == "Student")
        {
            var student = await GetApprovedStudentAsync(userId);

            if (student.StudentId != application.StudentId)
                throw new UnauthorizedAccessException("Access denied.");

            return;
        }

        if (role == "PlacementOfficer")
        {
            var collegeId = await GetApprovedOfficerCollegeIdAsync(userId);

            if (application.Student.CollegeId != collegeId)
                throw new UnauthorizedAccessException("Access denied.");

            return;
        }

        throw new UnauthorizedAccessException("Access denied.");
    }

    private async Task<InterviewSchedule> GetScheduleEntityAsync(int interviewId)
    {
        var schedule = await _context.InterviewSchedules
            .Include(s => s.Application)
                .ThenInclude(a => a.Job)
            .FirstOrDefaultAsync(s =>
                s.InterviewScheduleId == interviewId);

        return schedule ?? throw new KeyNotFoundException(
            "Interview schedule not found.");
    }

    private async Task<InterviewScheduleResponseDto> GetScheduleResponseAsync(
        int interviewId)
    {
        var schedule = await _context.InterviewSchedules
            .Include(s => s.Application)
                .ThenInclude(a => a.Job)
            .Include(s => s.Application)
                .ThenInclude(a => a.Student)
                    .ThenInclude(st => st.User)
            .Include(s => s.InterviewRound)
            .FirstAsync(s => s.InterviewScheduleId == interviewId);

        return MapSchedule(schedule);
    }

    private static InterviewRoundResponseDto MapRound(
        InterviewRound round, string jobTitle)
    {
        return new InterviewRoundResponseDto
        {
            InterviewRoundId = round.InterviewRoundId,
            JobId = round.JobId,
            JobTitle = jobTitle,
            RoundName = round.RoundName,
            RoundNumber = round.RoundNumber,
            Description = round.Description
        };
    }

    private static InterviewScheduleResponseDto MapSchedule(
        InterviewSchedule schedule)
    {
        return new InterviewScheduleResponseDto
        {
            InterviewScheduleId = schedule.InterviewScheduleId,
            ApplicationId = schedule.ApplicationId,
            JobId = schedule.Application.JobId,
            JobTitle = schedule.Application.Job.JobTitle,
            StudentId = schedule.Application.StudentId,
            StudentName = schedule.Application.Student.User.FullName,
            StudentEmail = schedule.Application.Student.User.Email ?? "",
            InterviewRoundId = schedule.InterviewRoundId,
            RoundName = schedule.InterviewRound.RoundName,
            RoundNumber = schedule.InterviewRound.RoundNumber,
            ScheduledAt = schedule.ScheduledAt,
            MeetingLink = schedule.MeetingLink,
            Venue = schedule.Venue,
            Status = schedule.Status,
            Feedback = schedule.Feedback
        };
    }
}
