using CampusPlacementManagement.Data;
using CampusPlacementManagement.Interfaces.Repositories;
using CampusPlacementManagement.Models.Entities;
using CampusPlacementManagement.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagement.Repositories;

public class JobOpeningRepository : IJobOpeningRepository
{
    private readonly ApplicationDbContext _context;

    public JobOpeningRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<JobOpening?> GetByIdAsync(int jobId)
    {
        return await _context.JobOpenings
            .Include(j => j.Company)
            .Include(j => j.CreatedByHR)
            .Include(j => j.EligibleDepartments)
            .Include(j => j.EligibleGraduationYears)
            .Include(j => j.JobTargetColleges)
            .FirstOrDefaultAsync(j => j.JobId == jobId);
    }

    public async Task<IEnumerable<JobOpening>> GetByCompanyIdAsync(int companyId)
    {
        return await _context.JobOpenings
            .Where(j => j.CompanyId == companyId)
            .Include(j => j.Company)
            .Include(j => j.EligibleDepartments)
            .Include(j => j.EligibleGraduationYears)
            .Include(j => j.JobTargetColleges)
            .OrderByDescending(j => j.CreatedAt)
            .ToListAsync();
    }

    public async Task AddAsync(JobOpening jobOpening)
    {
        await _context.JobOpenings.AddAsync(jobOpening);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(JobOpening jobOpening)
    {
        _context.JobOpenings.Update(jobOpening);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> CollegeExistsAsync(int collegeId)
    {
        return await _context.Colleges
            .AnyAsync(c => c.CollegeId == collegeId);
    }

    public async Task<bool> IsCollegeApprovedAsync(int collegeId)
    {
        return await _context.Colleges
            .AnyAsync(c =>
                c.CollegeId == collegeId &&
                c.Status == CollegeStatus.Approved);
    }
    public async Task RemoveTargetCollegeAsync(JobTargetCollege targetCollege)
    {
        _context.Remove(targetCollege);
        await _context.SaveChangesAsync();
    }
}
