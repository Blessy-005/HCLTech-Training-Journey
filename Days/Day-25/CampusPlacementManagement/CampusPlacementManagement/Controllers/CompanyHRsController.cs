using System.Security.Claims;
using CampusPlacementManagement.DTOs.CompanyHRs;
using CampusPlacementManagement.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusPlacementManagement.Controllers;

[Route("api/company-hrs")]
[ApiController]
public class CompanyHRsController : ControllerBase
{
    private readonly ICompanyHRService _companyHRService;

    public CompanyHRsController(ICompanyHRService companyHRService)
    {
        _companyHRService = companyHRService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterCompanyHRDto dto)
    {
        try
        {
            await _companyHRService.RegisterAsync(dto);

            return Ok(new
            {
                message = "Company HR registration submitted successfully. Awaiting approval."
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _companyHRService.GetAllAsync());
    }

    [HttpGet("pending")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> GetPending()
    {
        return Ok(await _companyHRService.GetPendingAsync());
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var companyHR = await _companyHRService.GetByIdAsync(id);

            if (companyHR == null)
                return NotFound(new { message = "Company HR not found." });

            return Ok(companyHR);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}/approve")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Approve(int id)
    {
        try
        {
            var approvedBy = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(approvedBy))
                return Unauthorized();

            var result = await _companyHRService.ApproveAsync(
                id, approvedBy);

            if (!result)
                return NotFound(new { message = "Company HR not found." });

            return Ok(new { message = "Company HR approved successfully." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}/reject")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Reject(int id)
    {
        try
        {
            var result = await _companyHRService.RejectAsync(id);

            if (!result)
                return NotFound(new { message = "Company HR not found." });

            return Ok(new { message = "Company HR rejected successfully." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}/suspend")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Suspend(int id)
    {
        try
        {
            var result = await _companyHRService.SuspendAsync(id);

            if (!result)
                return NotFound(new { message = "Company HR not found." });

            return Ok(new { message = "Company HR suspended successfully." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("me")]
    [Authorize(Roles = "CompanyHR")]
    public async Task<IActionResult> GetMe()
    {
        var userId = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        try
        {
            var companyHR =
                await _companyHRService.GetCurrentCompanyHRAsync(userId);

            if (companyHR == null)
                return NotFound(new { message = "Company HR profile not found." });

            return Ok(companyHR);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }
}