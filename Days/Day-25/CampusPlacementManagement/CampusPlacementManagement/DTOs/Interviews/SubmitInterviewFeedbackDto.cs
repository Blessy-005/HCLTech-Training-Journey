using CampusPlacementManagement.Models.Enums;

namespace CampusPlacementManagement.DTOs.Interviews;

public class SubmitInterviewFeedbackDto
{
    public string? Feedback { get; set; }
    public InterviewScheduleStatus Status { get; set; }
}