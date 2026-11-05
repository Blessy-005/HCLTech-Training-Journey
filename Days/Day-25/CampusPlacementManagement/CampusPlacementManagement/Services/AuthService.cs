using CampusPlacementManagement.DTOs.Authentication;
using CampusPlacementManagement.Helpers;
using CampusPlacementManagement.Interfaces.Services;
using CampusPlacementManagement.Models.Entities;
using CampusPlacementManagement.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace CampusPlacementManagement.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly JwtSettings _jwtSettings;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        IJwtTokenService jwtTokenService,
        IOptions<JwtSettings> jwtOptions)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
        _jwtSettings = jwtOptions.Value;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);

        if (user == null || user.AccountStatus != AccountStatus.Approved)
        {
            return null;
        }

        var passwordValid = await _userManager.CheckPasswordAsync(
            user, dto.Password);

        if (!passwordValid)
        {
            return null;
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtTokenService.GenerateToken(user, roles);

        return new LoginResponseDto
        {
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            Roles = roles
        };
    }
}
