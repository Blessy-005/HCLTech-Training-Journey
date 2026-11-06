using CampusPlacementManagement.DTOs.Applications;
using CampusPlacementManagement.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CampusPlacementManagement.Controllers;

[ApiController]
[Route("api/applications")]
[Authorize]
public class JobApplicationsController : ControllerBase
{
    private readonly IJobApplicationService _service;

    public JobApplicationsController(IJobApplicationService service)
    {
        _service = service;
    }

    private string? CurrentUserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier);

    private string CurrentRole =>
        User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

    [HttpPost]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Apply([FromBody] ApplyJobDto dto)
    {
        if (string.IsNullOrWhiteSpace(CurrentUserId))
            return Unauthorized();

        try
        {
            var result = await _service.ApplyAsync(dto, CurrentUserId);
            return CreatedAtAction(
                nameof(GetById),
                new { applicationId = result.ApplicationId },
                result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("my-applications")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetMyApplications()
    {
        if (string.IsNullOrWhiteSpace(CurrentUserId))
            return Unauthorized();

        try
        {
            return Ok(await _service.GetMyApplicationsAsync(CurrentUserId));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
    }

    [HttpGet("{applicationId:int}")]
    [Authorize(Roles = "Student,CompanyHR,PlacementOfficer,SuperAdmin")]
    public async Task<IActionResult> GetById(int applicationId)
    {
        if (string.IsNullOrWhiteSpace(CurrentUserId))
            return Unauthorized();

        try
        {
            return Ok(await _service.GetByIdAsync(
                applicationId, CurrentUserId, CurrentRole));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
    }

    [HttpPut("{applicationId:int}/withdraw")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Withdraw(int applicationId)
    {
        if (string.IsNullOrWhiteSpace(CurrentUserId))
            return Unauthorized();

        try
        {
            await _service.WithdrawAsync(applicationId, CurrentUserId);
            return Ok(new { message = "Application withdrawn successfully." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("/api/jobs/{jobId:int}/applications")]
    [Authorize(Roles = "CompanyHR,PlacementOfficer,SuperAdmin")]
    public async Task<IActionResult> GetForJob(int jobId)
    {
        if (string.IsNullOrWhiteSpace(CurrentUserId))
            return Unauthorized();

        try
        {
            return Ok(await _service.GetForJobAsync(
                jobId, CurrentUserId, CurrentRole));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
    }
}
