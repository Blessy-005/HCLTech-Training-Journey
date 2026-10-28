
using StudentCourseManagement.Models;
using StudentCourseManagement.Repository;

namespace StudentCourseManagement.Services;

public class AuditLogService
{
    private readonly AuditLogRepository _repository;

    public AuditLogService(AuditLogRepository repository)
    {
        _repository = repository;
    }

    public Task AddAsync(AuditLog auditLog)
    {
        return _repository.AddAsync(auditLog);
    }

    public Task<List<AuditLog>> GetRecentAsync(int count = 20)
    {
        return _repository.GetRecentAsync(count);
    }
}
