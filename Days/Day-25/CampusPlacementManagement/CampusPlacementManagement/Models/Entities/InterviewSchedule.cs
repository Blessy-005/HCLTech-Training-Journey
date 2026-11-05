using System.ComponentModel.DataAnnotations;
using CampusPlacementManagement.Models.Enums;

namespace CampusPlacementManagement.Models.Entities;

public class InterviewSchedule
{
    public int InterviewScheduleId { get; set; }

    public int ApplicationId { get; set; }

    public int InterviewRoundId { get; set; }

    public DateTime ScheduledAt { get; set; }

    [MaxLength(500)]
    public string? MeetingLink { get; set; }

    [MaxLength(255)]
    public string? Venue { get; set; }

    public InterviewScheduleStatus Status { get; set; }
        = InterviewScheduleStatus.Scheduled;

    public string? Feedback { get; set; }

    // Navigation properties
    public JobApplication Application { get; set; } = null!;

    public InterviewRound InterviewRound { get; set; } = null!;
}

