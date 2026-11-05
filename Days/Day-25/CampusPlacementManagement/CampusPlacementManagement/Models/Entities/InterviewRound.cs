using System.ComponentModel.DataAnnotations;

namespace CampusPlacementManagement.Models.Entities;

public class InterviewRound
{
    public int InterviewRoundId { get; set; }

    public int JobId { get; set; }

    [Required]
    [MaxLength(100)]
    public string RoundName { get; set; } = string.Empty;

    public int RoundNumber { get; set; }

    public string? Description { get; set; }

    public JobOpening Job { get; set; } = null!;
}
