using CampusPlacementManagement.Data;
using CampusPlacementManagement.DTOs.Applications;
using CampusPlacementManagement.Interfaces.Services;
using CampusPlacementManagement.Models.Entities;
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
        {
            throw new ArgumentException("Invalid application status.");
        }

        var application = await _context.JobApplications
            .FirstOrDefaultAsync(a => a.ApplicationId == applicationId);

        if (application == null)
        {
            throw new KeyNotFoundException(
                $"Application with ID {applicationId} was not found.");
        }

        var previousStatus = application.Status;

        application.Status = dto.NewStatus;
        application.LastUpdatedAt = DateTime.UtcNow;

        var history = new ApplicationStatusHistory
        {
            ApplicationId = applicationId,
            PreviousStatus = previousStatus,
            NewStatus = dto.NewStatus,
            UpdatedByUserId = updatedByUserId,
            Remarks = dto.Remarks,
            UpdatedAt = DateTime.UtcNow
        };

        _context.ApplicationStatusHistories.Add(history);

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<ApplicationStatusHistoryDto>> GetHistoryAsync(
        int applicationId)
    {
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
