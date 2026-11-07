namespace CampusPlacementManagement.DTOs.Interviews;

public class UpdateInterviewScheduleDto
{
    public DateTime ScheduledAt { get; set; }
    public string? MeetingLink { get; set; }
    public string? Venue { get; set; }
}