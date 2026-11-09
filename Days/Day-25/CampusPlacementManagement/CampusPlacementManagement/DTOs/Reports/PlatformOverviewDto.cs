namespace CampusPlacementManagement.DTOs.Reports;

public class PlatformOverviewDto
{
    public int TotalColleges { get; set; }
    public int ApprovedColleges { get; set; }

    public int TotalCompanies { get; set; }
    public int ApprovedCompanies { get; set; }

    public int TotalStudents { get; set; }
    public int ApprovedStudents { get; set; }

    public int TotalJobOpenings { get; set; }
    public int PublishedJobOpenings { get; set; }

    public int TotalApplications { get; set; }
    public int SelectedApplications { get; set; }
    public int PlacedStudents { get; set; }
}
