using CampusPlacementManagement.DTOs.Interviews;

namespace CampusPlacementManagement.Interfaces.Services;

public interface IInterviewService
{
    Task<InterviewRoundResponseDto> CreateRoundAsync(
        int jobId, CreateInterviewRoundDto dto, string userId);

    Task<IEnumerable<InterviewRoundResponseDto>> GetRoundsAsync(
        int jobId, string userId, string role);

    Task<InterviewScheduleResponseDto> ScheduleAsync(
        int applicationId, ScheduleInterviewDto dto, string userId);

    Task<IEnumerable<InterviewScheduleResponseDto>> GetInterviewsAsync(
        int applicationId, string userId, string role);

    Task<InterviewScheduleResponseDto?> UpdateAsync(
        int interviewId, UpdateInterviewScheduleDto dto, string userId);

    Task<InterviewScheduleResponseDto?> SubmitFeedbackAsync(
        int interviewId, SubmitInterviewFeedbackDto dto, string userId);
}