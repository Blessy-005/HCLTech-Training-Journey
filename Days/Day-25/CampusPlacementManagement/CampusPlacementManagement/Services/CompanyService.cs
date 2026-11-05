using CampusPlacementManagement.DTOs.Companies;
using CampusPlacementManagement.Interfaces.Repositories;
using CampusPlacementManagement.Interfaces.Services;
using CampusPlacementManagement.Models.Entities;
using CampusPlacementManagement.Models.Enums;

namespace CampusPlacementManagement.Services;

public class CompanyService : ICompanyService
{
    private readonly ICompanyRepository _companyRepository;

    public CompanyService(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<CompanyResponseDto> RegisterAsync(
        RegisterCompanyDto dto)
    {
        var email = dto.CompanyEmail.Trim();

        if (await _companyRepository.ExistsByEmailAsync(email))
        {
            throw new InvalidOperationException(
                "A company with this email already exists.");
        }

        var company = new Company
        {
            CompanyName = dto.CompanyName.Trim(),
            CompanyEmail = email,
            Website = dto.Website?.Trim(),
            Industry = dto.Industry.Trim(),
            Address = dto.Address.Trim(),
            Description = dto.Description?.Trim(),
            Status = CompanyStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _companyRepository.AddAsync(company);

        return MapToResponse(company);
    }

    public async Task<IEnumerable<CompanyResponseDto>> GetAllAsync()
    {
        var companies = await _companyRepository.GetAllAsync();

        return companies.Select(MapToResponse);
    }

    public async Task<IEnumerable<CompanyResponseDto>> GetPendingAsync()
    {
        var companies = await _companyRepository.GetPendingAsync();

        return companies.Select(MapToResponse);
    }

    public async Task<CompanyResponseDto?> GetByIdAsync(
        int id,
        string userId,
        bool isSuperAdmin)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id));

        var company = await _companyRepository.GetByIdAsync(id);

        if (company == null)
            return null;

        if (!isSuperAdmin &&
            !await _companyRepository.IsCompanyHRForCompanyAsync(
                userId, id))
        {
            throw new UnauthorizedAccessException(
                "You can access only your associated company.");
        }

        return MapToResponse(company);
    }

    public async Task<CompanyResponseDto?> UpdateAsync(
        int id,
        UpdateCompanyDto dto,
        string userId,
        bool isSuperAdmin)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id));

        var company = await _companyRepository.GetByIdAsync(id);

        if (company == null)
            return null;

        if (!isSuperAdmin &&
            !await _companyRepository.IsCompanyHRForCompanyAsync(
                userId, id))
        {
            throw new UnauthorizedAccessException(
                "You can update only your associated company.");
        }

        var email = dto.CompanyEmail.Trim();

        if (!string.Equals(
                company.CompanyEmail,
                email,
                StringComparison.OrdinalIgnoreCase) &&
            await _companyRepository.ExistsByEmailAsync(email))
        {
            throw new InvalidOperationException(
                "Another company is already registered with this email.");
        }

        company.CompanyName = dto.CompanyName.Trim();
        company.CompanyEmail = email;
        company.Website = dto.Website?.Trim();
        company.Industry = dto.Industry.Trim();
        company.Address = dto.Address.Trim();
        company.Description = dto.Description?.Trim();

        await _companyRepository.UpdateAsync(company);

        return MapToResponse(company);
    }

    public async Task<bool> ApproveAsync(int id)
    {
        var company = await _companyRepository.GetByIdAsync(id);

        if (company == null)
            return false;

        if (company.Status != CompanyStatus.Pending)
        {
            throw new InvalidOperationException(
                "Only pending companies can be approved.");
        }

        company.Status = CompanyStatus.Approved;

        await _companyRepository.UpdateAsync(company);

        return true;
    }

    public async Task<bool> RejectAsync(int id)
    {
        var company = await _companyRepository.GetByIdAsync(id);

        if (company == null)
            return false;

        if (company.Status != CompanyStatus.Pending)
        {
            throw new InvalidOperationException(
                "Only pending companies can be rejected.");
        }

        company.Status = CompanyStatus.Rejected;

        await _companyRepository.UpdateAsync(company);

        return true;
    }

    public async Task<bool> SuspendAsync(int id)
    {
        var company = await _companyRepository.GetByIdAsync(id);

        if (company == null)
            return false;

        if (company.Status != CompanyStatus.Approved)
        {
            throw new InvalidOperationException(
                "Only approved companies can be suspended.");
        }

        company.Status = CompanyStatus.Suspended;

        await _companyRepository.UpdateAsync(company);

        return true;
    }

    private static CompanyResponseDto MapToResponse(Company company)
    {
        return new CompanyResponseDto
        {
            CompanyId = company.CompanyId,
            CompanyName = company.CompanyName,
            CompanyEmail = company.CompanyEmail,
            Website = company.Website,
            Industry = company.Industry,
            Address = company.Address,
            Description = company.Description,
            Status = company.Status.ToString(),
            CreatedAt = company.CreatedAt
        };
    }
}
