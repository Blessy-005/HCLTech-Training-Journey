using System.Security.Claims;
using CampusPlacementManagement.DTOs.Jobs;
using CampusPlacementManagement.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusPlacementManagement.Controllers;

[ApiController]
[Route("api/jobs")]
[Authorize(Roles = "CompanyHR")]
public class JobOpeningsController : ControllerBase
{
    private readonly IJobOpeningService _jobService;

    public JobOpeningsController(IJobOpeningService jobService)
    {
        _jobService = jobService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateJobOpeningDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        try
        {
            var job = await _jobService.CreateAsync(dto, userId);

            return Ok(new
            {
                message = "Job opening created successfully as Draft.",
                job
            });
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

    [HttpGet]
    public async Task<IActionResult> GetMyJobs()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        try
        {
            return Ok(await _jobService.GetMyJobsAsync(userId));
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
            return BadRequest(new { message = "Invalid job ID." });

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        try
        {
            var job = await _jobService.GetByIdAsync(id, userId);

            return job == null
                ? NotFound(new { message = "Job opening not found." })
                : Ok(job);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateJobOpeningDto dto)
    {
        if (id <= 0)
            return BadRequest(new { message = "Invalid job ID." });

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        try
        {
            var job = await _jobService.UpdateAsync(id, dto, userId);

            return job == null
                ? NotFound(new { message = "Job opening not found." })
                : Ok(job);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}/publish")]
    public async Task<IActionResult> Publish(int id)
    {
        if (id <= 0)
            return BadRequest(new { message = "Invalid job ID." });

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        try
        {
            var result = await _jobService.PublishAsync(id, userId);

            return result
                ? Ok(new { message = "Job published successfully." })
                : NotFound(new { message = "Job opening not found." });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}/close")]
    public async Task<IActionResult> Close(int id)
    {
        if (id <= 0)
            return BadRequest(new { message = "Invalid job ID." });

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        try
        {
            var result = await _jobService.CloseAsync(id, userId);

            return result
                ? Ok(new { message = "Job closed successfully." })
                : NotFound(new { message = "Job opening not found." });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id:int}/target-colleges")]
    public async Task<IActionResult> SetTargetColleges(
        int id,
        [FromBody] SetJobTargetCollegesDto dto)
    {
        if (id <= 0)
            return BadRequest(new { message = "Invalid job ID." });

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        try
        {
            var result = await _jobService.SetTargetCollegesAsync(
                id, dto, userId);

            return result
                ? Ok(new { message = "Target colleges updated successfully." })
                : NotFound(new { message = "Job opening not found." });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id:int}/target-colleges")]
    public async Task<IActionResult> GetTargetColleges(int id)
    {
        if (id <= 0)
            return BadRequest(new { message = "Invalid job ID." });

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        try
        {
            var colleges = await _jobService.GetTargetCollegesAsync(id, userId);

            return colleges == null
                ? NotFound(new { message = "Job opening not found." })
                : Ok(colleges);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}/target-colleges/{collegeId:int}")]
    public async Task<IActionResult> RemoveTargetCollege(
        int id,
        int collegeId)
    {
        if (id <= 0 || collegeId <= 0)
            return BadRequest(new { message = "Invalid job or college ID." });

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        try
        {
            var result = await _jobService.RemoveTargetCollegeAsync(
                id, collegeId, userId);

            return result
                ? Ok(new { message = "Target college removed successfully." })
                : NotFound(new { message = "Job or target college not found." });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
