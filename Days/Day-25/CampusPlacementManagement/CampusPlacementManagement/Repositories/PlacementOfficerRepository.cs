using CampusPlacementManagement.Data;
using CampusPlacementManagement.Interfaces.Repositories;
using CampusPlacementManagement.Models.Entities;
using CampusPlacementManagement.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagement.Repositories;

public class PlacementOfficerRepository : IPlacementOfficerRepository
{
    private readonly ApplicationDbContext _context;

    public PlacementOfficerRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PlacementOfficer?> GetByIdAsync(int id)
    {
        return await _context.PlacementOfficers
            .Include(p => p.User)
            .Include(p => p.College)
            .FirstOrDefaultAsync(p => p.PlacementOfficerId == id);
    }

    public async Task<PlacementOfficer?> GetByUserIdAsync(string userId)
    {
        return await _context.PlacementOfficers
            .Include(p => p.User)
            .Include(p => p.College)
            .FirstOrDefaultAsync(p => p.UserId == userId);
    }

    public async Task<IEnumerable<PlacementOfficer>> GetAllAsync()
    {
        return await _context.PlacementOfficers
            .Include(p => p.User)
            .Include(p => p.College)
            .OrderBy(p => p.PlacementOfficerId)
            .ToListAsync();
    }

    public async Task<IEnumerable<PlacementOfficer>> GetPendingAsync()
    {
        return await _context.PlacementOfficers
            .Include(p => p.User)
            .Include(p => p.College)
            .Where(p => p.Status == PlacementOfficerStatus.Pending)
            .OrderBy(p => p.PlacementOfficerId)
            .ToListAsync();
    }

    public async Task AddAsync(PlacementOfficer placementOfficer)
    {
        await _context.PlacementOfficers.AddAsync(placementOfficer);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(PlacementOfficer placementOfficer)
    {
        _context.PlacementOfficers.Update(placementOfficer);
        await _context.SaveChangesAsync();
    }
}
