using System.ComponentModel.DataAnnotations;

namespace CampusPlacementManagement.DTOs.Jobs;

public class SetJobTargetCollegesDto
{
    [Required]
    [MinLength(1)]
    public List<int> CollegeIds { get; set; } = new();
}
