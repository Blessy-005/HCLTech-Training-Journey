using CampusPlacementManagement.DTOs.Colleges;
using CampusPlacementManagement.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusPlacementManagement.Controllers;

[ApiController]
[Route("api/colleges")]
[Authorize]
public class CollegesController : ControllerBase
{
    private readonly ICollegeService _collegeService;

    public CollegesController(ICollegeService collegeService)
    {
        _collegeService = collegeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var colleges = await _collegeService.GetAllAsync();
        return Ok(colleges);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var college = await _collegeService.GetByIdAsync(id);

            if (college == null)
                return NotFound(new { message = "College not found." });

            return Ok(college);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "SuperAdmin")]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateCollegeDto dto)
    {
        try
        {
            var college = await _collegeService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = college.CollegeId },
                college);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "SuperAdmin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateCollegeDto dto)
    {
        try
        {
            var updated = await _collegeService.UpdateAsync(id, dto);

            if (!updated)
                return NotFound(new { message = "College not found." });

            return Ok(new { message = "College updated successfully." });
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "SuperAdmin")]
    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(
        int id,
        [FromBody] UpdateCollegeStatusDto dto)
    {
        try
        {
            var updated = await _collegeService.UpdateStatusAsync(id, dto);

            if (!updated)
                return NotFound(new { message = "College not found." });

            return Ok(new
            {
                message = "College status updated successfully."
            });
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
