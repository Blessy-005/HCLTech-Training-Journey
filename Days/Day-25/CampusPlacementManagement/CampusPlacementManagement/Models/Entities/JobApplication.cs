using CampusPlacementManagement.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace CampusPlacementManagement.Models.Entities;

public class JobApplication
{
    public int ApplicationId { get; set; }

    public int JobId { get; set; }

    public int StudentId { get; set; }

    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

    public JobApplicationStatus Status { get; set; }
        = JobApplicationStatus.Applied;

    [MaxLength(500)]
    public string? ResumeUrl { get; set; }

    public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public JobOpening Job { get; set; } = null!;

    public Student Student { get; set; } = null!;
}