namespace CampusPlacementManagement.DTOs.Interviews;

public class ScheduleInterviewDto
{
    public int InterviewRoundId { get; set; }
    public DateTime ScheduledAt { get; set; }
    public string? MeetingLink { get; set; }
    public string? Venue { get; set; }
}