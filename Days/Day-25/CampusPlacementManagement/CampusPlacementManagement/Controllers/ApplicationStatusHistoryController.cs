using CampusPlacementManagement.DTOs.Applications;
using CampusPlacementManagement.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CampusPlacementManagement.Controllers;

[ApiController]
[Route("api/applications")]
[Authorize]
public class ApplicationStatusHistoryController : ControllerBase
{
    private readonly IApplicationStatusHistoryService _service;

    public ApplicationStatusHistoryController(
        IApplicationStatusHistoryService service)
    {
        _service = service;
    }

    [HttpPut("{applicationId:int}/status")]
    public async Task<IActionResult> UpdateStatus(
        int applicationId,
        [FromBody] UpdateApplicationStatusDto dto)
    {
        var updatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(updatedByUserId))
        {
            return Unauthorized();
        }

        try
        {
            await _service.UpdateStatusAsync(
                applicationId,
                dto,
                updatedByUserId);

            return Ok(new
            {
                message = "Application status updated successfully."
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{applicationId:int}/status-history")]
    public async Task<IActionResult> GetHistory(int applicationId)
    {
        var history = await _service.GetHistoryAsync(applicationId);

        return Ok(history);
    }
}
