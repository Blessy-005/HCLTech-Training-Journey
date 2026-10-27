
using StudentCourseManagement.Models;
using StudentCourseManagement.Repository;

namespace StudentCourseManagement.Services;

public class EnrollmentService
{
    private readonly EnrollmentRepository _repository;

    public EnrollmentService(EnrollmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> EnrollAsync(Enrollment enrollment)
    {
        ValidateEnrollment(enrollment);

        return await _repository.AddAsync(enrollment);
    }

    public Task<List<Enrollment>> GetAllAsync()
    {
        return _repository.GetAllAsync();
    }

    public async Task<Enrollment?> GetByIdAsync(int enrollmentId)
    {
        if (enrollmentId <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(enrollmentId),
                "Enrollment ID must be greater than zero.");

        return await _repository.GetByIdAsync(enrollmentId);
    }

    public async Task<bool> UpdateStatusAsync(
        int enrollmentId, string status)
    {
        if (enrollmentId <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(enrollmentId),
                "Enrollment ID must be greater than zero.");

        if (string.IsNullOrWhiteSpace(status))
            throw new ArgumentException(
                "Enrollment status is required.");

        status = status.Trim();

        if (status is not ("Enrolled" or "Completed" or "Cancelled"))
            throw new ArgumentException(
                "Status must be Enrolled, Completed, or Cancelled.");

        return await _repository.UpdateStatusAsync(
            enrollmentId, status);
    }

    private static void ValidateEnrollment(Enrollment enrollment)
    {
        ArgumentNullException.ThrowIfNull(enrollment);

        if (enrollment.StudentId <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(enrollment.StudentId),
                "Student ID must be positive.");

        if (enrollment.CourseId <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(enrollment.CourseId),
                "Course ID must be positive.");

        if (string.IsNullOrWhiteSpace(enrollment.Status))
            throw new ArgumentException(
                "Enrollment status is required.");

        enrollment.Status = enrollment.Status.Trim();

        if (enrollment.Status is not ("Enrolled" or "Completed" or "Cancelled"))
            throw new ArgumentException(
                "Invalid enrollment status.");
    }
}
