using CampusPlacementManagement.DTOs.Authentication;
using CampusPlacementManagement.Interfaces.Services;
using CampusPlacementManagement.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CampusPlacementManagement.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthController(
        IAuthService authService,
        UserManager<ApplicationUser> userManager)
    {
        _authService = authService;
        _userManager = userManager;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);

        if (result == null)
        {
            return Unauthorized(new
            {
                message = "Invalid credentials or account is not approved."
            });
        }

        return Ok(result);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            return NotFound();

        var roles = await _userManager.GetRolesAsync(user);

        return Ok(new
        {
            user.Id,
            user.FullName,
            user.Email,
            user.AccountStatus,
            Roles = roles
        });
    }
}
