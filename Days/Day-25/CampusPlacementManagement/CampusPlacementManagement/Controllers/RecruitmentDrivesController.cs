using System.Security.Claims;
using CampusPlacementManagement.DTOs.RecruitmentDrives;
using CampusPlacementManagement.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusPlacementManagement.Controllers;

[ApiController]
[Route("api/recruitment-drives")]
[Authorize(Roles = "CompanyHR,PlacementOfficer,Student,SuperAdmin")]
public class RecruitmentDrivesController : ControllerBase
{
    private readonly IRecruitmentDriveService _service;

    public RecruitmentDrivesController(IRecruitmentDriveService service)
    {
        _service = service;
    }

    private string? CurrentUserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier);

    private string? CurrentRole =>
        User.FindFirstValue(ClaimTypes.Role);

    [HttpPost]
    [Authorize(Roles = "CompanyHR")]
    public async Task<IActionResult> Create(
        [FromBody] CreateRecruitmentDriveDto dto)
    {
        if (string.IsNullOrEmpty(CurrentUserId))
            return Unauthorized();

        try
        {
            var drive = await _service.CreateAsync(dto, CurrentUserId);

            return CreatedAtAction(
                nameof(GetById),
                new { id = drive.DriveId },
                drive);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        if (string.IsNullOrEmpty(CurrentUserId) ||
            string.IsNullOrEmpty(CurrentRole))
            return Unauthorized();

        try
        {
            return Ok(await _service.GetVisibleAsync(
                CurrentUserId, CurrentRole));
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        if (id <= 0)
            return BadRequest(new { message = "Invalid recruitment drive ID." });

        if (string.IsNullOrEmpty(CurrentUserId) ||
            string.IsNullOrEmpty(CurrentRole))
            return Unauthorized();

        try
        {
            var drive = await _service.GetByIdAsync(
                id, CurrentUserId, CurrentRole);

            return drive == null
                ? NotFound(new { message = "Recruitment drive not found." })
                : Ok(drive);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "CompanyHR")]
    public async Task<IActionResult> Update(
        int id, [FromBody] UpdateRecruitmentDriveDto dto)
    {
        if (id <= 0)
            return BadRequest(new { message = "Invalid recruitment drive ID." });

        if (string.IsNullOrEmpty(CurrentUserId))
            return Unauthorized();

        try
        {
            var drive = await _service.UpdateAsync(id, dto, CurrentUserId);

            return drive == null
                ? NotFound(new { message = "Recruitment drive not found." })
                : Ok(drive);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}/cancel")]
    [Authorize(Roles = "CompanyHR")]
    public async Task<IActionResult> Cancel(int id)
    {
        if (id <= 0)
            return BadRequest(new { message = "Invalid recruitment drive ID." });

        if (string.IsNullOrEmpty(CurrentUserId))
            return Unauthorized();

        try
        {
            var result = await _service.CancelAsync(id, CurrentUserId);

            return result
                ? Ok(new { message = "Recruitment drive cancelled successfully." })
                : NotFound(new { message = "Recruitment drive not found." });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
