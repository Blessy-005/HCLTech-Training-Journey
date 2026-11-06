using CampusPlacementManagement.Models.Entities;

namespace CampusPlacementManagement.Interfaces.Repositories;

public interface IStudentRepository
{
    Task<Student?> GetByIdAsync(int id);

    Task<Student?> GetByUserIdAsync(string userId);

    Task<Student?> GetByRegisterNumberAsync(int collegeId, string registerNumber);

    Task<IEnumerable<Student>> GetByCollegeAsync(int collegeId);

    Task<IEnumerable<Student>> GetPendingByCollegeAsync(int collegeId);

    Task AddAsync(Student student);

    Task UpdateAsync(Student student);
}
