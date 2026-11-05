
using System.ComponentModel.DataAnnotations;
using CampusPlacementManagement.Models.Enums;

namespace CampusPlacementManagement.Models.Entities;

public class RecruitmentDrive
{
    public int DriveId { get; set; }

    public int JobId { get; set; }

    public int CollegeId { get; set; }

    public DateTime DriveDate { get; set; }

    [MaxLength(255)]
    public string? Venue { get; set; }

    public string? Description { get; set; }

    public RecruitmentDriveStatus Status { get; set; }
        = RecruitmentDriveStatus.Scheduled;

    // Navigation properties
    public JobOpening Job { get; set; } = null!;

    public College College { get; set; } = null!;
}
