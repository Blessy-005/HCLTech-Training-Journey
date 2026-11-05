using CampusPlacementManagement.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace CampusPlacementManagement.Models.Entities;

public class ApplicationStatusHistory
{
    public int HistoryId { get; set; }

    public int ApplicationId { get; set; }

    public JobApplicationStatus? PreviousStatus { get; set; }

    public JobApplicationStatus NewStatus { get; set; }

    public string UpdatedByUserId { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Remarks { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public JobApplication Application { get; set; } = null!;

    public ApplicationUser UpdatedByUser { get; set; } = null!;
}
