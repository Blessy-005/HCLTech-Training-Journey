namespace CampusPlacementManagement.DTOs.Reports;

public class StudentPlacementDto
{
    public int CollegeId { get; set; }
    public string CollegeName { get; set; } = string.Empty;

    public int TotalStudents { get; set; }
    public int ApprovedStudents { get; set; }

    public int TotalApplications { get; set; }
    public int SelectedApplications { get; set; }
    public int PlacedStudents { get; set; }

    public List<StudentPlacementItemDto> Students { get; set; } = new();
}

public class StudentPlacementItemDto
{
    public int StudentId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string RegisterNumber { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public int GraduationYear { get; set; }
    public decimal CGPA { get; set; }
    public string Status { get; set; } = string.Empty;

    public int TotalApplications { get; set; }
    public int SelectedApplications { get; set; }
    public DateTime? LastAppliedAt { get; set; }
}
