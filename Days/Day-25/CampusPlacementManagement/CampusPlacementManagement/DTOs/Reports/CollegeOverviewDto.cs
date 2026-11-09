namespace CampusPlacementManagement.DTOs.Reports;

public class CollegeOverviewDto
{
    public int CollegeId { get; set; }
    public string CollegeName { get; set; } = string.Empty;
    public string CollegeCode { get; set; } = string.Empty;

    public int TotalStudents { get; set; }
    public int ApprovedStudents { get; set; }

    public int TotalApplications { get; set; }
    public int SelectedApplications { get; set; }
    public int PlacedStudents { get; set; }

    public int AvailableJobs { get; set; }
    public int TotalRecruitmentDrives { get; set; }
}
