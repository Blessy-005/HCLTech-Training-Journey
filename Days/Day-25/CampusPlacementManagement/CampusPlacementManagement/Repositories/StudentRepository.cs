using CampusPlacementManagement.Data;
using CampusPlacementManagement.Interfaces.Repositories;
using CampusPlacementManagement.Models.Entities;
using CampusPlacementManagement.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagement.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly ApplicationDbContext _context;

    public StudentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Student?> GetByIdAsync(int id)
    {
        return await _context.Students
            .Include(s => s.User)
            .Include(s => s.College)
            .Include(s => s.PlacementOfficer)
            .FirstOrDefaultAsync(s => s.StudentId == id);
    }

    public async Task<Student?> GetByUserIdAsync(string userId)
    {
        return await _context.Students
            .Include(s => s.User)
            .Include(s => s.College)
            .Include(s => s.PlacementOfficer)
            .FirstOrDefaultAsync(s => s.UserId == userId);
    }

    public async Task<Student?> GetByRegisterNumberAsync(int collegeId, string registerNumber)
    {
        return await _context.Students
            .FirstOrDefaultAsync(s =>
                s.CollegeId == collegeId &&
                s.RegisterNumber == registerNumber);
    }

    public async Task<IEnumerable<Student>> GetByCollegeAsync(int collegeId)
    {
        return await _context.Students
            .Include(s => s.User)
            .Include(s => s.College)
            .Include(s => s.PlacementOfficer)
            .Where(s => s.CollegeId == collegeId)
            .OrderBy(s => s.StudentId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Student>> GetPendingByCollegeAsync(int collegeId)
    {
        return await _context.Students
            .Include(s => s.User)
            .Include(s => s.College)
            .Include(s => s.PlacementOfficer)
            .Where(s =>
                s.CollegeId == collegeId &&
                s.Status == StudentStatus.Pending)
            .OrderBy(s => s.StudentId)
            .ToListAsync();
    }

    public async Task AddAsync(Student student)
    {
        await _context.Students.AddAsync(student);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Student student)
    {
        _context.Students.Update(student);
        await _context.SaveChangesAsync();
    }
}
