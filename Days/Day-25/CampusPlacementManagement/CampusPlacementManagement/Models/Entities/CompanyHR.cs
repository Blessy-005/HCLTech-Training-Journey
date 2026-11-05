using System.ComponentModel.DataAnnotations;
using CampusPlacementManagement.Models.Enums;

namespace CampusPlacementManagement.Models.Entities;

public class CompanyHR
{
    public int CompanyHRId { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    public int CompanyId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Designation { get; set; } = string.Empty;

    public CompanyHRStatus Status { get; set; }
        = CompanyHRStatus.Pending;

    public string? ApprovedBy { get; set; }

    public DateTime? ApprovedAt { get; set; }

    // Navigation properties
    public ApplicationUser User { get; set; } = null!;

    public Company Company { get; set; } = null!;

    public ApplicationUser? Approver { get; set; }
}