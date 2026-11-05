namespace CampusPlacementManagement.Models.Entities;

public class JobTargetCollege
{
    public int JobTargetCollegeId { get; set; }

    public int JobId { get; set; }

    public int CollegeId { get; set; }

    // Navigation properties
    public JobOpening Job { get; set; } = null!;

    public College College { get; set; } = null!;
}