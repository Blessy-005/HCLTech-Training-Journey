using CampusPlacementManagement.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace CampusPlacementManagement.DTOs.Applications;

public class UpdateApplicationStatusDto
{
    [Required]
    public JobApplicationStatus NewStatus { get; set; }

    [MaxLength(500)]
    public string? Remarks { get; set; }
}
