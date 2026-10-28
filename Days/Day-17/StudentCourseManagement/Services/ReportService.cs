
using StudentCourseManagement.Repository;

namespace StudentCourseManagement.Services;

public class ReportService
{
    private readonly ReportRepository _repository;

    public ReportService(ReportRepository repository)
    {
        _repository = repository;
    }

    public Task<List<(string DepartmentName, int StudentCount)>>
        GetStudentCountByDepartmentAsync()
    {
        return _repository.GetStudentCountByDepartmentAsync();
    }

    public Task<List<(string CourseName, int EnrollmentCount)>>
        GetEnrollmentCountByCourseAsync()
    {
        return _repository.GetEnrollmentCountByCourseAsync();
    }

    public Task<List<(int StudentId, string Name)>>
        GetStudentsWithoutEnrollmentsAsync()
    {
        return _repository.GetStudentsWithoutEnrollmentsAsync();
    }

    public Task<List<(int CourseId, string CourseName)>>
        GetCoursesWithoutEnrollmentsAsync()
    {
        return _repository.GetCoursesWithoutEnrollmentsAsync();
    }
}
