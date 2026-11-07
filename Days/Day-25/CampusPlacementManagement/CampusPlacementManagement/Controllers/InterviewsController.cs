
using System.Security.Claims;
using CampusPlacementManagement.DTOs.Interviews;
using CampusPlacementManagement.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusPlacementManagement.Controllers;

[ApiController]
[Route("api")]
[Authorize(Roles = "CompanyHR,PlacementOfficer,Student,SuperAdmin")]
public class InterviewsController : ControllerBase
{
    private readonly IInterviewService _interviewService;

    public InterviewsController(IInterviewService interviewService)
    {
        _interviewService = interviewService;
    }

    private string CurrentUserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new UnauthorizedAccessException("User ID not found.");

    private string CurrentRole =>
        User.FindFirstValue(ClaimTypes.Role)
        ?? throw new UnauthorizedAccessException("User role not found.");

    [HttpPost("jobs/{jobId:int}/interview-rounds")]
    [Authorize(Roles = "CompanyHR")]
    public async Task<IActionResult> CreateRound(
        int jobId, CreateInterviewRoundDto dto)
    {
        try
        {
            var result = await _interviewService.CreateRoundAsync(
                jobId, dto, CurrentUserId);

            return Created(
                $"/api/jobs/{jobId}/interview-rounds/{result.InterviewRoundId}",
                result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex) when (
            ex is ArgumentException || ex is InvalidOperationException)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("jobs/{jobId:int}/interview-rounds")]
    public async Task<IActionResult> GetRounds(int jobId)
    {
        try
        {
            var result = await _interviewService.GetRoundsAsync(
                jobId, CurrentUserId, CurrentRole);

            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpPost("applications/{applicationId:int}/interviews")]
    [Authorize(Roles = "CompanyHR")]
    public async Task<IActionResult> Schedule(
        int applicationId, ScheduleInterviewDto dto)
    {
        try
        {
            var result = await _interviewService.ScheduleAsync(
                applicationId, dto, CurrentUserId);

            return CreatedAtAction(
                nameof(GetInterviews),
                new { applicationId },
                result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex) when (
            ex is ArgumentException || ex is InvalidOperationException)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("applications/{applicationId:int}/interviews")]
    public async Task<IActionResult> GetInterviews(int applicationId)
    {
        try
        {
            var result = await _interviewService.GetInterviewsAsync(
                applicationId, CurrentUserId, CurrentRole);

            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpPut("interviews/{id:int}")]
    [Authorize(Roles = "CompanyHR")]
    public async Task<IActionResult> Update(
        int id, UpdateInterviewScheduleDto dto)
    {
        try
        {
            var result = await _interviewService.UpdateAsync(
                id, dto, CurrentUserId);

            return result == null ? NotFound() : Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex) when (
            ex is ArgumentException || ex is InvalidOperationException)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("interviews/{id:int}/feedback")]
    [Authorize(Roles = "CompanyHR")]
    public async Task<IActionResult> SubmitFeedback(
        int id, SubmitInterviewFeedbackDto dto)
    {
        try
        {
            var result = await _interviewService.SubmitFeedbackAsync(
                id, dto, CurrentUserId);

            return result == null ? NotFound() : Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex) when (
            ex is ArgumentException || ex is InvalidOperationException)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
