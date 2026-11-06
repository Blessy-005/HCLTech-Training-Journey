using CampusPlacementManagement.Data;
using CampusPlacementManagement.DTOs.Applications;
using CampusPlacementManagement.Interfaces.Services;
using CampusPlacementManagement.Models.Entities;
using CampusPlacementManagement.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagement.Services;

public class ApplicationStatusHistoryService
    : IApplicationStatusHistoryService
{
    private readonly ApplicationDbContext _context;

    public ApplicationStatusHistoryService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> UpdateStatusAsync(
        int applicationId,
        UpdateApplicationStatusDto dto,
        string updatedByUserId)
    {
        if (!Enum.IsDefined(dto.NewStatus))
            throw new ArgumentException("Invalid application status.");

        if (string.IsNullOrWhiteSpace(updatedByUserId))
            throw new UnauthorizedAccessException("User identity is required.");

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == updatedByUserId);

        if (user == null || user.AccountStatus != AccountStatus.Approved)
            throw new UnauthorizedAccessException("Your account is not approved.");

        var roles = await (
            from userRole in _context.UserRoles
            join role in _context.Roles
                on userRole.RoleId equals role.Id
            where userRole.UserId == updatedByUserId
            select role.Name
        ).ToListAsync();

        var application = await _context.JobApplications
            .Include(a => a.Job)
            .Include(a => a.Student)
            .FirstOrDefaultAsync(a => a.ApplicationId == applicationId);

        if (application == null)
            throw new KeyNotFoundException(
                $"Application with ID {applicationId} was not found.");

        var previousStatus = application.Status;
        var authorized = false;
        var isSuperAdmin = roles.Contains("SuperAdmin");

        if (isSuperAdmin)
        {
            authorized = true;
        }
        else if (roles.Contains("CompanyHR"))
        {
            authorized = dto.NewStatus != JobApplicationStatus.Withdrawn
                && dto.NewStatus != JobApplicationStatus.Applied
                && await _context.CompanyHRs.AnyAsync(hr =>
                    hr.UserId == updatedByUserId &&
                    hr.CompanyId == application.Job.CompanyId &&
                    hr.Status == CompanyHRStatus.Approved &&
                    hr.User.AccountStatus == AccountStatus.Approved &&
                    hr.Company.Status == CompanyStatus.Approved);
        }
        else if (roles.Contains("Student") &&
                 dto.NewStatus == JobApplicationStatus.Withdrawn)
        {
            authorized = await _context.Students.AnyAsync(student =>
                student.UserId == updatedByUserId &&
                student.StudentId == application.StudentId &&
                student.Status == StudentStatus.Approved &&
                student.User.AccountStatus == AccountStatus.Approved &&
                student.College.Status == CollegeStatus.Approved);
        }

        if (!authorized)
            throw new UnauthorizedAccessException(
                "You are not authorized to update this application.");

        if (previousStatus == dto.NewStatus)
            throw new ArgumentException(
                "The application already has this status.");

        if (!IsAllowedTransition(previousStatus, dto.NewStatus, isSuperAdmin))
            throw new ArgumentException(
                $"Changing status from {previousStatus} to {dto.NewStatus} is not allowed.");

        var now = DateTime.UtcNow;

        application.Status = dto.NewStatus;
        application.LastUpdatedAt = now;

        _context.ApplicationStatusHistories.Add(
            new ApplicationStatusHistory
            {
                ApplicationId = applicationId,
                PreviousStatus = previousStatus,
                NewStatus = dto.NewStatus,
                UpdatedByUserId = updatedByUserId,
                Remarks = dto.Remarks,
                UpdatedAt = now
            });

        await _context.SaveChangesAsync();

        return true;
    }

    private static bool IsAllowedTransition(
        JobApplicationStatus current,
        JobApplicationStatus next,
        bool isSuperAdmin)
    {
        if (current is JobApplicationStatus.Rejected
            or JobApplicationStatus.Selected
            or JobApplicationStatus.NotSelected
            or JobApplicationStatus.Withdrawn)
            return false;

        if (next == JobApplicationStatus.Withdrawn)
            return isSuperAdmin || current is
                JobApplicationStatus.Applied
                or JobApplicationStatus.UnderReview
                or JobApplicationStatus.Shortlisted
                or JobApplicationStatus.InterviewScheduled;

        return current switch
        {
            JobApplicationStatus.Applied =>
                next is JobApplicationStatus.UnderReview
                    or JobApplicationStatus.Shortlisted
                    or JobApplicationStatus.Rejected,

            JobApplicationStatus.UnderReview =>
                next is JobApplicationStatus.Shortlisted
                    or JobApplicationStatus.Rejected
                    or JobApplicationStatus.InterviewScheduled,

            JobApplicationStatus.Shortlisted =>
                next is JobApplicationStatus.InterviewScheduled
                    or JobApplicationStatus.Rejected
                    or JobApplicationStatus.Selected
                    or JobApplicationStatus.NotSelected,

            JobApplicationStatus.InterviewScheduled =>
                next is JobApplicationStatus.Rejected
                    or JobApplicationStatus.Selected
                    or JobApplicationStatus.NotSelected,

            _ => false
        };
    }

    public async Task<List<ApplicationStatusHistoryDto>> GetHistoryAsync(
        int applicationId,
        string requestedByUserId)
    {
        if (string.IsNullOrWhiteSpace(requestedByUserId))
            throw new UnauthorizedAccessException("User identity is required.");

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == requestedByUserId);

        if (user == null || user.AccountStatus != AccountStatus.Approved)
            throw new UnauthorizedAccessException("Your account is not approved.");

        var application = await _context.JobApplications
            .Include(a => a.Job)
            .FirstOrDefaultAsync(a => a.ApplicationId == applicationId);

        if (application == null)
            throw new KeyNotFoundException(
                $"Application with ID {applicationId} was not found.");

        var roles = await (
            from userRole in _context.UserRoles
            join role in _context.Roles
                on userRole.RoleId equals role.Id
            where userRole.UserId == requestedByUserId
            select role.Name
        ).ToListAsync();

        var authorized = roles.Contains("SuperAdmin");

        if (!authorized && roles.Contains("Student"))
        {
            authorized = await _context.Students.AnyAsync(student =>
                student.UserId == requestedByUserId &&
                student.StudentId == application.StudentId &&
                student.Status == StudentStatus.Approved &&
                student.User.AccountStatus == AccountStatus.Approved &&
                student.College.Status == CollegeStatus.Approved);
        }

        if (!authorized && roles.Contains("CompanyHR"))
        {
            authorized = await _context.CompanyHRs.AnyAsync(hr =>
                hr.UserId == requestedByUserId &&
                hr.CompanyId == application.Job.CompanyId &&
                hr.Status == CompanyHRStatus.Approved &&
                hr.User.AccountStatus == AccountStatus.Approved &&
                hr.Company.Status == CompanyStatus.Approved);
        }

        if (!authorized && roles.Contains("PlacementOfficer"))
        {
            authorized = await _context.PlacementOfficers.AnyAsync(officer =>
                officer.UserId == requestedByUserId &&
                officer.Status == PlacementOfficerStatus.Approved &&
                officer.User.AccountStatus == AccountStatus.Approved &&
                officer.College.Status == CollegeStatus.Approved &&
                officer.CollegeId == application.Student.CollegeId);
        }

        if (!authorized)
            throw new UnauthorizedAccessException(
                "You are not authorized to view this application history.");

        return await _context.ApplicationStatusHistories
            .Where(h => h.ApplicationId == applicationId)
            .OrderByDescending(h => h.UpdatedAt)
            .Select(h => new ApplicationStatusHistoryDto
            {
                HistoryId = h.HistoryId,
                ApplicationId = h.ApplicationId,
                PreviousStatus = h.PreviousStatus,
                NewStatus = h.NewStatus,
                UpdatedByUserName = h.UpdatedByUser.FullName,
                Remarks = h.Remarks,
                UpdatedAt = h.UpdatedAt
            })
            .ToListAsync();
    }
}
