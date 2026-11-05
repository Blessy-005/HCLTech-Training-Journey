using System.Security.Claims;
using CampusPlacementManagement.DTOs.Companies;
using CampusPlacementManagement.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusPlacementManagement.Controllers;

[ApiController]
[Route("api/companies")]
public class CompaniesController : ControllerBase
{
    private readonly ICompanyService _companyService;

    public CompaniesController(ICompanyService companyService)
    {
        _companyService = companyService;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterCompanyDto dto)
    {
        try
        {
            var company = await _companyService.RegisterAsync(dto);

            return Ok(new
            {
                message = "Company registration submitted successfully. Await SuperAdmin approval.",
                company
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "SuperAdmin")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var companies = await _companyService.GetAllAsync();
        return Ok(companies);
    }

    [Authorize(Roles = "SuperAdmin")]
    [HttpGet("pending")]
    public async Task<IActionResult> GetPending()
    {
        var companies = await _companyService.GetPendingAsync();
        return Ok(companies);
    }

    [Authorize(Roles = "SuperAdmin,CompanyHR")]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        if (id <= 0)
            return BadRequest(new { message = "Invalid company ID." });

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        try
        {
            var company = await _companyService.GetByIdAsync(
                id,
                userId,
                User.IsInRole("SuperAdmin"));

            return company == null
                ? NotFound(new { message = "Company not found." })
                : Ok(company);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(
                StatusCodes.Status403Forbidden,
                new { message = ex.Message });
        }
    }

    [Authorize(Roles = "SuperAdmin,CompanyHR")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateCompanyDto dto)
    {
        if (id <= 0)
            return BadRequest(new { message = "Invalid company ID." });

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        try
        {
            var company = await _companyService.UpdateAsync(
                id,
                dto,
                userId,
                User.IsInRole("SuperAdmin"));

            return company == null
                ? NotFound(new { message = "Company not found." })
                : Ok(company);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(
                StatusCodes.Status403Forbidden,
                new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "SuperAdmin")]
    [HttpPut("{id:int}/approve")]
    public async Task<IActionResult> Approve(int id)
    {
        try
        {
            var result = await _companyService.ApproveAsync(id);

            if (!result)
                return NotFound(new { message = "Company not found." });

            return Ok(new
            {
                message = "Company approved successfully."
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
            var result = await _companyService.RejectAsync(id);

            if (!result)
                return NotFound(new { message = "Company not found." });

            return Ok(new
            {
                message = "Company rejected successfully."
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
            var result = await _companyService.SuspendAsync(id);

            if (!result)
                return NotFound(new { message = "Company not found." });

            return Ok(new
            {
                message = "Company suspended successfully."
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
