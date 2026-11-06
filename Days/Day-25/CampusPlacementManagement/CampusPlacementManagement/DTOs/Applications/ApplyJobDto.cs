using System.ComponentModel.DataAnnotations;

namespace CampusPlacementManagement.DTOs.Applications;

public class ApplyJobDto
{
    [Range(1, int.MaxValue)]
    public int JobId { get; set; }

    [MaxLength(500)]
    [Url]
    public string? ResumeUrl { get; set; }
}
