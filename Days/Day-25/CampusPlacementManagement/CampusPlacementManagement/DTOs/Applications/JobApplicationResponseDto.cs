using CampusPlacementManagement.Models.Enums;

namespace CampusPlacementManagement.DTOs.Applications;

public class JobApplicationResponseDto
{
    public int ApplicationId { get; set; }
    public int JobId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentEmail { get; set; } = string.Empty;
    public int CollegeId { get; set; }
    public string CollegeName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public int GraduationYear { get; set; }
    public decimal CGPA { get; set; }
    public string? ResumeUrl { get; set; }
    public DateTime AppliedAt { get; set; }
    public JobApplicationStatus Status { get; set; }
    public DateTime LastUpdatedAt { get; set; }
}
