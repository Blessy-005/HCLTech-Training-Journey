using System.Security.Claims;
using CampusPlacementManagement.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusPlacementManagement.Controllers;

[ApiController]
[Route("api/reports")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IReportsService _reportsService;

    public ReportsController(IReportsService reportsService)
    {
        _reportsService = reportsService;
    }

    [HttpGet("platform-overview")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> GetPlatformOverview()
    {
        var report = await _reportsService.GetPlatformOverviewAsync();
        return Ok(report);
    }

    [HttpGet("college-overview")]
    [Authorize(Roles = "PlacementOfficer")]
    public async Task<IActionResult> GetCollegeOverview()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var report = await _reportsService.GetCollegeOverviewAsync(userId);
        return Ok(report);
    }

    [HttpGet("company-recruitment")]
    [Authorize(Roles = "CompanyHR")]
    public async Task<IActionResult> GetCompanyRecruitment()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var report = await _reportsService.GetCompanyRecruitmentAsync(userId);
        return Ok(report);
    }

    [HttpGet("student-placement")]
    [Authorize(Roles = "PlacementOfficer")]
    public async Task<IActionResult> GetStudentPlacement()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var report = await _reportsService.GetStudentPlacementAsync(userId);
        return Ok(report);
    }
}
