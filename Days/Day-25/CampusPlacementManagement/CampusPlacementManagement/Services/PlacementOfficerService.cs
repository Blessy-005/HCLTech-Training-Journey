using CampusPlacementManagement.DTOs.PlacementOfficers;
using CampusPlacementManagement.Interfaces.Repositories;
using CampusPlacementManagement.Interfaces.Services;
using CampusPlacementManagement.Models.Entities;
using CampusPlacementManagement.Models.Enums;
using Microsoft.AspNetCore.Identity;

namespace CampusPlacementManagement.Services;

public class PlacementOfficerService : IPlacementOfficerService
{
    private readonly IPlacementOfficerRepository _placementOfficerRepository;
    private readonly ICollegeService _collegeService;
    private readonly UserManager<ApplicationUser> _userManager;

    public PlacementOfficerService(
        IPlacementOfficerRepository placementOfficerRepository,
        ICollegeService collegeService,
        UserManager<ApplicationUser> userManager)
    {
        _placementOfficerRepository = placementOfficerRepository;
        _collegeService = collegeService;
        _userManager = userManager;
    }

    public async Task<bool> RegisterAsync(RegisterPlacementOfficerDto dto)
    {
        var college = await _collegeService.GetByIdAsync(dto.CollegeId);

        if (college == null)
            throw new InvalidOperationException("College not found.");

        if (college.Status != CollegeStatus.Approved)
            throw new InvalidOperationException(
                "Registration is allowed only for approved colleges.");

        var email = dto.Email.Trim();

        var existingUser = await _userManager.FindByEmailAsync(email);

        if (existingUser != null)
            throw new InvalidOperationException(
                "An account with this email already exists.");

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            PhoneNumber = dto.MobileNumber.Trim(),
            FullName = dto.FullName.Trim(),
            AccountStatus = AccountStatus.Pending,
            EmailConfirmed = false
        };

        var userResult = await _userManager.CreateAsync(
            user, dto.Password);

        if (!userResult.Succeeded)
        {
            var errors = string.Join(
                ", ", userResult.Errors.Select(e => e.Description));

            throw new InvalidOperationException(
                $"Officer registration failed: {errors}");
        }

        var placementOfficer = new PlacementOfficer
        {
            UserId = user.Id,
            CollegeId = dto.CollegeId,
            Designation = dto.Designation.Trim(),
            Status = PlacementOfficerStatus.Pending
        };

        try
        {
            await _placementOfficerRepository.AddAsync(placementOfficer);
        }
        catch
        {
            await _userManager.DeleteAsync(user);
            throw;
        }

        return true;
    }

    public async Task<IEnumerable<object>> GetAllAsync()
    {
        var officers = await _placementOfficerRepository.GetAllAsync();

        return officers.Select(MapToResponse);
    }

    public async Task<IEnumerable<object>> GetPendingAsync()
    {
        var officers = await _placementOfficerRepository.GetPendingAsync();

        return officers.Select(MapToResponse);
    }

    public async Task<object?> GetByIdAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id));

        var officer = await _placementOfficerRepository.GetByIdAsync(id);

        return officer == null ? null : MapToResponse(officer);
    }

    public async Task<bool> ApproveAsync(int id, string approvedBy)
    {
        var officer = await _placementOfficerRepository.GetByIdAsync(id);

        if (officer == null)
            return false;

        if (officer.Status != PlacementOfficerStatus.Pending)
            throw new InvalidOperationException(
                "Only pending placement officers can be approved.");

        if (officer.College.Status != CollegeStatus.Approved)
            throw new InvalidOperationException(
                "The officer's college must be approved and active.");

        var user = await _userManager.FindByIdAsync(officer.UserId);

        if (user == null)
            throw new InvalidOperationException(
                "Officer user account not found.");

        var roleResult = await _userManager.AddToRoleAsync(
            user, "PlacementOfficer");

        if (!roleResult.Succeeded)
        {
            var errors = string.Join(
                ", ", roleResult.Errors.Select(e => e.Description));

            throw new InvalidOperationException(
                $"Role assignment failed: {errors}");
        }

        user.AccountStatus = AccountStatus.Approved;
        user.EmailConfirmed = true;
        user.UpdatedAt = DateTime.UtcNow;

        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            await _userManager.RemoveFromRoleAsync(
                user, "PlacementOfficer");

            throw new InvalidOperationException(
                "Failed to update officer account status.");
        }

        officer.Status = PlacementOfficerStatus.Approved;
        officer.ApprovedBy = approvedBy;
        officer.ApprovedAt = DateTime.UtcNow;

        await _placementOfficerRepository.UpdateAsync(officer);

        return true;
    }

    public async Task<bool> RejectAsync(int id)
    {
        var officer = await _placementOfficerRepository.GetByIdAsync(id);

        if (officer == null)
            return false;

        if (officer.Status != PlacementOfficerStatus.Pending)
            throw new InvalidOperationException(
                "Only pending placement officers can be rejected.");

        officer.Status = PlacementOfficerStatus.Rejected;

        var user = await _userManager.FindByIdAsync(officer.UserId);

        if (user != null)
        {
            user.AccountStatus = AccountStatus.Rejected;
            user.UpdatedAt = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                throw new InvalidOperationException(
                    "Failed to update officer account status.");
        }

        await _placementOfficerRepository.UpdateAsync(officer);

        return true;
    }

    public async Task<bool> SuspendAsync(int id)
    {
        var officer = await _placementOfficerRepository.GetByIdAsync(id);

        if (officer == null)
            return false;

        if (officer.Status != PlacementOfficerStatus.Approved)
            throw new InvalidOperationException(
                "Only approved placement officers can be suspended.");

        var user = await _userManager.FindByIdAsync(officer.UserId);

        if (user == null)
            throw new InvalidOperationException(
                "Officer user account not found.");

        user.AccountStatus = AccountStatus.Suspended;
        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
            throw new InvalidOperationException(
                "Failed to suspend officer account.");

        officer.Status = PlacementOfficerStatus.Suspended;

        await _placementOfficerRepository.UpdateAsync(officer);

        return true;
    }

    public async Task<object?> GetCurrentOfficerAsync(string userId)
    {
        var officer =
            await _placementOfficerRepository.GetByUserIdAsync(userId);

        if (officer == null)
            return null;

        if (officer.Status != PlacementOfficerStatus.Approved ||
            officer.User.AccountStatus != AccountStatus.Approved ||
            officer.College.Status != CollegeStatus.Approved)
        {
            throw new UnauthorizedAccessException(
                "Your account or college is not currently approved.");
        }

        return MapToResponse(officer);
    }

    private static PlacementOfficerResponseDto MapToResponse(
        PlacementOfficer officer)
    {
        return new PlacementOfficerResponseDto
        {
            PlacementOfficerId = officer.PlacementOfficerId,
            UserId = officer.UserId,
            FullName = officer.User.FullName,
            Email = officer.User.Email ?? string.Empty,
            MobileNumber = officer.User.PhoneNumber ?? string.Empty,
            CollegeId = officer.CollegeId,
            CollegeName = officer.College.CollegeName,
            CollegeCode = officer.College.CollegeCode,
            Designation = officer.Designation,
            Status = officer.Status.ToString(),
            ApprovedBy = officer.ApprovedBy,
            ApprovedAt = officer.ApprovedAt
        };
    }
}
