using CampusPlacementManagement.Data;
using CampusPlacementManagement.Interfaces.Repositories;
using CampusPlacementManagement.Models.Entities;
using CampusPlacementManagement.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagement.Repositories;

public class CompanyRepository : ICompanyRepository
{
    private readonly ApplicationDbContext _context;

    public CompanyRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Company?> GetByIdAsync(int id)
    {
        return await _context.Companies
            .FirstOrDefaultAsync(c => c.CompanyId == id);
    }

    public async Task<IEnumerable<Company>> GetAllAsync()
    {
        return await _context.Companies
            .OrderBy(c => c.CompanyId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Company>> GetPendingAsync()
    {
        return await _context.Companies
            .Where(c => c.Status == CompanyStatus.Pending)
            .OrderBy(c => c.CompanyId)
            .ToListAsync();
    }

    public async Task<bool> IsCompanyHRForCompanyAsync(
        string userId,
        int companyId)
    {
        return await _context.CompanyHRs
            .AnyAsync(h =>
                h.UserId == userId &&
                h.CompanyId == companyId &&
                h.Status == CompanyHRStatus.Approved &&
                h.User.AccountStatus == AccountStatus.Approved);
    }

    public async Task<bool> ExistsByEmailAsync(string companyEmail)
    {
        return await _context.Companies
            .AnyAsync(c => c.CompanyEmail == companyEmail);
    }

    public async Task AddAsync(Company company)
    {
        await _context.Companies.AddAsync(company);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Company company)
    {
        _context.Companies.Update(company);
        await _context.SaveChangesAsync();
    }
}

