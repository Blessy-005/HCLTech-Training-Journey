using CampusPlacementManagement.Data;
using CampusPlacementManagement.Interfaces.Repositories;
using CampusPlacementManagement.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagement.Repositories;

public class CollegeRepository : ICollegeRepository
{
    private readonly ApplicationDbContext _context;

    public CollegeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<College?> GetByIdAsync(int collegeId)
    {
        return await _context.Colleges
            .FirstOrDefaultAsync(c => c.CollegeId == collegeId);
    }

    public async Task<List<College>> GetAllAsync()
    {
        return await _context.Colleges
            .OrderBy(c => c.CollegeName)
            .ToListAsync();
    }

    public async Task<College?> GetByCodeAsync(string collegeCode)
    {
        return await _context.Colleges
            .FirstOrDefaultAsync(c => c.CollegeCode == collegeCode);
    }

    public async Task AddAsync(College college)
    {
        await _context.Colleges.AddAsync(college);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(College college)
    {
        _context.Colleges.Update(college);
        await _context.SaveChangesAsync();
    }
}
