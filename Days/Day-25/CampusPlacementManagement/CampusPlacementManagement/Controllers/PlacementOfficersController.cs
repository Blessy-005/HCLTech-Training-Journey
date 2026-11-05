using System.Security.Claims;
using CampusPlacementManagement.DTOs.PlacementOfficers;
using CampusPlacementManagement.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusPlacementManagement.Controllers;

[ApiController]
[Route("api/placement-officers")]
public class PlacementOfficersController : ControllerBase
{
    private readonly IPlacementOfficerService _placementOfficerService;

    public PlacementOfficersController(
        IPlacementOfficerService placementOfficerService)
    {
        _placementOfficerService = placementOfficerService;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterPlacementOfficerDto dto)
    {
        try
        {
            await _placementOfficerService.RegisterAsync(dto);

            return Ok(new
            {
                message = "Registration submitted successfully. Await SuperAdmin approval."
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "SuperAdmin")]
    [HttpGet("pending")]
    public async Task<IActionResult> GetPending()
    {
        var officers = await _placementOfficerService.GetPendingAsync();
        return Ok(officers);
    }

    [Authorize(Roles = "SuperAdmin")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var officers = await _placementOfficerService.GetAllAsync();
        return Ok(officers);
    }

    [Authorize(Roles = "SuperAdmin,PlacementOfficer")]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        if (id <= 0)
            return BadRequest(new { message = "Invalid officer ID." });

        if (User.IsInRole("SuperAdmin"))
        {
            var officer = await _placementOfficerService.GetByIdAsync(id);

            return officer == null
                ? NotFound(new { message = "Placement officer not found." })
                : Ok(officer);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        try
        {
            var currentOfficer =
                await _placementOfficerService.GetCurrentOfficerAsync(userId);

            if (currentOfficer is not PlacementOfficerResponseDto response ||
                response.PlacementOfficerId != id)
            {
                return Forbid();
            }

            return Ok(currentOfficer);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden,
                new { message = ex.Message });
        }
    }

    [Authorize(Roles = "SuperAdmin")]
    [HttpPut("{id:int}/approve")]
    public async Task<IActionResult> Approve(int id)
    {
        try
        {
            var approvedBy = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(approvedBy))
                return Unauthorized();

            var result = await _placementOfficerService.ApproveAsync(
                id, approvedBy);

            if (!result)
                return NotFound(new
                {
                    message = "Placement officer not found."
                });

            return Ok(new
            {
                message = "Placement officer approved successfully."
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "SuperAdmin")]
    [HttpPut("{id:int}/reject")]
    public async Task<IActionResult> Reject(int id)
    {
        try
        {
            var result = await _placementOfficerService.RejectAsync(id);

            if (!result)
                return NotFound(new
                {
                    message = "Placement officer not found."
                });

            return Ok(new
            {
                message = "Placement officer rejected successfully."
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "SuperAdmin")]
    [HttpPut("{id:int}/suspend")]
    public async Task<IActionResult> Suspend(int id)
    {
        try
        {
            var result = await _placementOfficerService.SuspendAsync(id);

            if (!result)
                return NotFound(new
                {
                    message = "Placement officer not found."
                });

            return Ok(new
            {
                message = "Placement officer suspended successfully."
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "PlacementOfficer")]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentOfficer()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        try
        {
            var officer =
                await _placementOfficerService.GetCurrentOfficerAsync(userId);

            if (officer == null)
                return NotFound(new
                {
                    message = "Placement officer profile not found."
                });

            return Ok(officer);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden,
                new { message = ex.Message });
        }
    }
}
