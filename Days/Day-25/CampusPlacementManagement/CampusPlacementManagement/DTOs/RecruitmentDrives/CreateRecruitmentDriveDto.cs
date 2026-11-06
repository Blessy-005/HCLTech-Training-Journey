using CampusPlacementManagement.Models.Enums;

namespace CampusPlacementManagement.DTOs.RecruitmentDrives;

public class CreateRecruitmentDriveDto
{
    public int JobId { get; set; }
    public int CollegeId { get; set; }
    public DateTime DriveDate { get; set; }
    public string? Venue { get; set; }
    public string? Description { get; set; }
}
