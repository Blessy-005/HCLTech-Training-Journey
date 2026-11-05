using System.ComponentModel.DataAnnotations;
using CampusPlacementManagement.Models.Enums;

namespace CampusPlacementManagement.Models.Entities;

public class PlacementOfficer
{
    public int PlacementOfficerId { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    public int CollegeId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Designation { get; set; } = string.Empty;

    public PlacementOfficerStatus Status { get; set; }
        = PlacementOfficerStatus.Pending;

    public string? ApprovedBy { get; set; }

    public DateTime? ApprovedAt { get; set; }

    // Navigation properties
    public ApplicationUser User { get; set; } = null!;

    public College College { get; set; } = null!;

    public ApplicationUser? Approver { get; set; }
}