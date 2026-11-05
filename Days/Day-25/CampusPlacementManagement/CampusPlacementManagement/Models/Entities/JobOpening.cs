using System.ComponentModel.DataAnnotations;
using CampusPlacementManagement.Models.Enums;

namespace CampusPlacementManagement.Models.Entities;

public class JobOpening
{
    public int JobId { get; set; }

    public int CompanyId { get; set; }

    public int CreatedByHRId { get; set; }

    [Required]
    [MaxLength(200)]
    public string JobTitle { get; set; } = string.Empty;

    [Required]
    public string JobDescription { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string JobType { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Location { get; set; } = string.Empty;

    public decimal? SalaryPackage { get; set; }

    public decimal? MinimumCGPA { get; set; }

    public int? MaximumBacklogs { get; set; }

    public DateTime ApplicationDeadline { get; set; }

    public bool IsOpenToAllColleges { get; set; }

    public JobStatus Status { get; set; } = JobStatus.Draft;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Company Company { get; set; } = null!;

    public CompanyHR CreatedByHR { get; set; } = null!;
}