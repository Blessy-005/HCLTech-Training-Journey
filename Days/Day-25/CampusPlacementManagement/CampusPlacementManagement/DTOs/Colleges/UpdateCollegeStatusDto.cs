using System.ComponentModel.DataAnnotations;
using CampusPlacementManagement.Models.Enums;

namespace CampusPlacementManagement.DTOs.Colleges;

public class UpdateCollegeStatusDto
{
    [Required]
    public CollegeStatus Status { get; set; }
}
