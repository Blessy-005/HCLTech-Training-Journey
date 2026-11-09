namespace CampusPlacementManagement.DTOs.Reports;

public class CompanyRecruitmentDto
{
    public int CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;

    public int TotalJobOpenings { get; set; }
    public int PublishedJobOpenings { get; set; }

    public int TotalApplications { get; set; }
    public int SelectedApplications { get; set; }
    public int PlacedStudents { get; set; }

    public int TotalInterviewSchedules { get; set; }
}
