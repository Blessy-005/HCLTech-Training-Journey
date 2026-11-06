using CampusPlacementManagement.Data;
using CampusPlacementManagement.Interfaces.Repositories;
using CampusPlacementManagement.Models.Entities;
using CampusPlacementManagement.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagement.Repositories;

public class CompanyHRRepository : ICompanyHRRepository
{
    private readonly ApplicationDbContext _context;

    public CompanyHRRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CompanyHR?> GetByIdAsync(int id)
    {
        return await _context.CompanyHRs
            .Include(h => h.User)
            .Include(h => h.Company)
            .Include(h => h.Approver)
            .FirstOrDefaultAsync(h => h.CompanyHRId == id);
    }

    public async Task<CompanyHR?> GetByUserIdAsync(string userId)
    {
        return await _context.CompanyHRs
            .Include(h => h.User)
            .Include(h => h.Company)
            .Include(h => h.Approver)
            .FirstOrDefaultAsync(h => h.UserId == userId);
    }

    public async Task<IEnumerable<CompanyHR>> GetAllAsync()
    {
        return await _context.CompanyHRs
            .Include(h => h.User)
            .Include(h => h.Company)
            .Include(h => h.Approver)
            .OrderBy(h => h.CompanyHRId)
            .ToListAsync();
    }

    public async Task<IEnumerable<CompanyHR>> GetPendingAsync()
    {
        return await _context.CompanyHRs
            .Include(h => h.User)
            .Include(h => h.Company)
            .Include(h => h.Approver)
            .Where(h => h.Status == CompanyHRStatus.Pending)
            .OrderBy(h => h.CompanyHRId)
            .ToListAsync();
    }

    public async Task AddAsync(CompanyHR companyHR)
    {
        await _context.CompanyHRs.AddAsync(companyHR);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(CompanyHR companyHR)
    {
        _context.CompanyHRs.Update(companyHR);
        await _context.SaveChangesAsync();
    }
}