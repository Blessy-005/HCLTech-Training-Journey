using CampusPlacementManagement.DTOs.CompanyHRs;
using CampusPlacementManagement.Interfaces.Repositories;
using CampusPlacementManagement.Interfaces.Services;
using CampusPlacementManagement.Models.Entities;
using CampusPlacementManagement.Models.Enums;
using CampusPlacementManagement.Repositories;
using Microsoft.AspNetCore.Identity;

namespace CampusPlacementManagement.Services;

public class CompanyHRService : ICompanyHRService
{
    private readonly ICompanyHRRepository _companyHRRepository;
    private readonly ICompanyService _companyService;
    private readonly UserManager<ApplicationUser> _userManager;

    public CompanyHRService(
        ICompanyHRRepository companyHRRepository,
        ICompanyService companyService,
        UserManager<ApplicationUser> userManager)
    {
        _companyHRRepository = companyHRRepository;
        _companyService = companyService;
        _userManager = userManager;
    }

    public async Task<bool> RegisterAsync(RegisterCompanyHRDto dto)
    {
        var company = await _companyService.GetByIdAsync(
            dto.CompanyId, string.Empty, true);

        if (company == null)
            throw new KeyNotFoundException("Company not found.");

        if (company.Status != CompanyStatus.Approved.ToString())
            throw new InvalidOperationException(
                "Only approved companies can register Company HR users.");

        var email = dto.Email.Trim();

        if (await _userManager.FindByEmailAsync(email) != null)
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

        var identityResult = await _userManager.CreateAsync(
            user, dto.Password);

        if (!identityResult.Succeeded)
        {
            var errors = string.Join(
                "; ",
                identityResult.Errors.Select(e => e.Description));

            throw new InvalidOperationException(errors);
        }

        try
        {
            var companyHR = new CompanyHR
            {
                UserId = user.Id,
                CompanyId = dto.CompanyId,
                Designation = dto.Designation.Trim(),
                Status = CompanyHRStatus.Pending
            };

            await _companyHRRepository.AddAsync(companyHR);
            return true;
        }
        catch
        {
            await _userManager.DeleteAsync(user);
            throw;
        }
    }

    public async Task<IEnumerable<CompanyHRResponseDto>> GetAllAsync()
    {
        var companyHRs = await _companyHRRepository.GetAllAsync();

        return companyHRs.Select(MapToResponse);
    }

    public async Task<IEnumerable<CompanyHRResponseDto>> GetPendingAsync()
    {
        var companyHRs = await _companyHRRepository.GetPendingAsync();

        return companyHRs.Select(MapToResponse);
    }

    public async Task<CompanyHRResponseDto?> GetByIdAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(id), "Company HR ID must be greater than zero.");

        var companyHR = await _companyHRRepository.GetByIdAsync(id);

        return companyHR == null ? null : MapToResponse(companyHR);
    }

    public async Task<bool> ApproveAsync(int id, string approvedBy)
    {
        var companyHR = await _companyHRRepository.GetByIdAsync(id);

        if (companyHR == null)
            return false;

        if (companyHR.Status != CompanyHRStatus.Pending)
            throw new InvalidOperationException(
                "Only pending Company HR accounts can be approved.");

        if (companyHR.Company.Status != CompanyStatus.Approved)
            throw new InvalidOperationException(
                "The associated company must be approved.");

        var user = await _userManager.FindByIdAsync(companyHR.UserId);

        if (user == null)
            throw new InvalidOperationException(
                "Company HR user account not found.");

        var roleResult = await _userManager.AddToRoleAsync(
            user, "CompanyHR");

        if (!roleResult.Succeeded)
        {
            var errors = string.Join(
                "; ",
                roleResult.Errors.Select(e => e.Description));

            throw new InvalidOperationException(errors);
        }

        user.AccountStatus = AccountStatus.Approved;
        user.EmailConfirmed = true;
        user.UpdatedAt = DateTime.UtcNow;

        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            await _userManager.RemoveFromRoleAsync(user, "CompanyHR");

            var errors = string.Join(
                "; ",
                updateResult.Errors.Select(e => e.Description));

            throw new InvalidOperationException(errors);
        }

        companyHR.Status = CompanyHRStatus.Approved;
        companyHR.ApprovedBy = approvedBy;
        companyHR.ApprovedAt = DateTime.UtcNow;

        await _companyHRRepository.UpdateAsync(companyHR);

        return true;
    }

    public async Task<bool> RejectAsync(int id)
    {
        var companyHR = await _companyHRRepository.GetByIdAsync(id);

        if (companyHR == null)
            return false;

        if (companyHR.Status != CompanyHRStatus.Pending)
            throw new InvalidOperationException(
                "Only pending Company HR accounts can be rejected.");

        var user = await _userManager.FindByIdAsync(companyHR.UserId);

        if (user == null)
            throw new InvalidOperationException(
                "Company HR user account not found.");

        user.AccountStatus = AccountStatus.Rejected;
        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
            throw new InvalidOperationException(
                "Unable to update the user account status.");

        companyHR.Status = CompanyHRStatus.Rejected;

        await _companyHRRepository.UpdateAsync(companyHR);

        return true;
    }

    public async Task<bool> SuspendAsync(int id)
    {
        var companyHR = await _companyHRRepository.GetByIdAsync(id);

        if (companyHR == null)
            return false;

        if (companyHR.Status != CompanyHRStatus.Approved)
            throw new InvalidOperationException(
                "Only approved Company HR accounts can be suspended.");

        var user = await _userManager.FindByIdAsync(companyHR.UserId);

        if (user == null)
            throw new InvalidOperationException(
                "Company HR user account not found.");

        user.AccountStatus = AccountStatus.Suspended;
        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
            throw new InvalidOperationException(
                "Unable to suspend the user account.");

        companyHR.Status = CompanyHRStatus.Suspended;

        await _companyHRRepository.UpdateAsync(companyHR);

        return true;
    }

    public async Task<CompanyHRResponseDto?> GetCurrentCompanyHRAsync(
        string userId)
    {
        var companyHR = await _companyHRRepository.GetByUserIdAsync(userId);

        if (companyHR == null)
            return null;

        if (companyHR.Status != CompanyHRStatus.Approved ||
            companyHR.User.AccountStatus != AccountStatus.Approved ||
            companyHR.Company.Status != CompanyStatus.Approved)
        {
            throw new UnauthorizedAccessException(
                "Your Company HR account is not currently active.");
        }

        return MapToResponse(companyHR);
    }

    private static CompanyHRResponseDto MapToResponse(CompanyHR companyHR)
    {
        return new CompanyHRResponseDto
        {
            CompanyHRId = companyHR.CompanyHRId,
            UserId = companyHR.UserId,
            FullName = companyHR.User.FullName,
            Email = companyHR.User.Email ?? string.Empty,
            MobileNumber = companyHR.User.PhoneNumber ?? string.Empty,
            CompanyId = companyHR.CompanyId,
            CompanyName = companyHR.Company.CompanyName,
            Designation = companyHR.Designation,
            Status = companyHR.Status.ToString(),
            ApprovedBy = companyHR.ApprovedBy,
            ApprovedAt = companyHR.ApprovedAt
        };
    }
}