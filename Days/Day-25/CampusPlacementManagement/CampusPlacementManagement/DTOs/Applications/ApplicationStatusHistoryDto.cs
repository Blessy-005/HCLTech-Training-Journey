using CampusPlacementManagement.Models.Enums;

namespace CampusPlacementManagement.DTOs.Applications;

public class ApplicationStatusHistoryDto
{
    public int HistoryId { get; set; }

    public int ApplicationId { get; set; }

    public JobApplicationStatus? PreviousStatus { get; set; }

    public JobApplicationStatus NewStatus { get; set; }

    public string UpdatedByUserName { get; set; } = string.Empty;

    public string? Remarks { get; set; }

    public DateTime UpdatedAt { get; set; }
}
