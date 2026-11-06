using CampusPlacementManagement.Data;
using CampusPlacementManagement.Interfaces.Repositories;
using CampusPlacementManagement.Models.Entities;
using CampusPlacementManagement.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagement.Repositories;

public class RecruitmentDriveRepository : IRecruitmentDriveRepository
{
    private readonly ApplicationDbContext _context;

    public RecruitmentDriveRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RecruitmentDrive?> GetByIdAsync(int driveId)
    {
        return await _context.RecruitmentDrives
            .Include(d => d.Job)
                .ThenInclude(j => j.Company)
            .Include(d => d.College)
            .FirstOrDefaultAsync(d => d.DriveId == driveId);
    }

    public async Task<IEnumerable<RecruitmentDrive>> GetAllAsync()
    {
        return await _context.RecruitmentDrives
            .Include(d => d.Job)
                .ThenInclude(j => j.Company)
            .Include(d => d.College)
            .OrderBy(d => d.DriveDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<RecruitmentDrive>> GetByCompanyIdAsync(int companyId)
    {
        return await _context.RecruitmentDrives
            .Where(d => d.Job.CompanyId == companyId)
            .Include(d => d.Job)
                .ThenInclude(j => j.Company)
            .Include(d => d.College)
            .OrderBy(d => d.DriveDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<RecruitmentDrive>> GetByCollegeIdAsync(int collegeId)
    {
        return await _context.RecruitmentDrives
            .Where(d => d.CollegeId == collegeId)
            .Include(d => d.Job)
                .ThenInclude(j => j.Company)
            .Include(d => d.College)
            .OrderBy(d => d.DriveDate)
            .ToListAsync();
    }

    public async Task<JobOpening?> GetJobByIdAsync(int jobId)
    {
        return await _context.JobOpenings
            .Include(j => j.Company)
            .Include(j => j.JobTargetColleges)
            .FirstOrDefaultAsync(j => j.JobId == jobId);
    }

    public async Task<bool> IsCollegeApprovedAsync(int collegeId)
    {
        return await _context.Colleges.AnyAsync(c =>
            c.CollegeId == collegeId &&
            c.Status == CollegeStatus.Approved);
    }

    public async Task AddAsync(RecruitmentDrive drive)
    {
        await _context.RecruitmentDrives.AddAsync(drive);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(RecruitmentDrive drive)
    {
        _context.RecruitmentDrives.Update(drive);
        await _context.SaveChangesAsync();
    }
}
