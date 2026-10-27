
using StudentCourseManagement.Models;
using StudentCourseManagement.Repository;

namespace StudentCourseManagement.Services;

public class CourseService
{
    private readonly CourseRepository _repository;

    public CourseService(CourseRepository repository)
    {
        _repository = repository;
    }

    public async Task AddCourseAsync(Course course)
    {
        ValidateCourse(course);

        if (course.DepartmentId <= 0)
            throw new ArgumentException(
                "Select a valid department.");

        await _repository.AddCourseAsync(course);
    }

    public Task<List<Course>> GetAllCoursesAsync()
    {
        return _repository.GetAllCoursesAsync();
    }

    public async Task<Course?> GetCourseByIdAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException(
                "Course ID must be greater than zero.");

        return await _repository.GetCourseByIdAsync(id);
    }

    public async Task<List<Course>> GetCoursesByDepartmentAsync(
        int departmentId)
    {
        if (departmentId <= 0)
            throw new ArgumentException(
                "Department ID must be greater than zero.");

        return await _repository.GetCoursesByDepartmentAsync(departmentId);
    }

    public async Task UpdateCourseAsync(Course course)
    {
        if (course.CourseId <= 0)
            throw new ArgumentException("Invalid course ID.");

        ValidateCourse(course);

        if (course.DepartmentId <= 0)
            throw new ArgumentException("Invalid department ID.");

        bool updated = await _repository.UpdateCourseAsync(course);

        if (!updated)
            throw new InvalidOperationException(
                "Course not found or already inactive.");
    }

    public async Task DeactivateCourseAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("Invalid course ID.");

        bool deactivated = await _repository.DeactivateCourseAsync(id);

        if (!deactivated)
            throw new InvalidOperationException(
                "Course not found or already inactive.");
    }

    private static void ValidateCourse(Course course)
    {
        if (string.IsNullOrWhiteSpace(course.CourseName))
            throw new ArgumentException("Course name is required.");

        if (course.CourseName.Length > 100)
            throw new ArgumentException(
                "Course name cannot exceed 100 characters.");

        if (course.DurationMonths <= 0)
            throw new ArgumentException(
                "Duration must be greater than zero.");

        if (course.Fee < 0 || course.Fee > 99999999.99m)
            throw new ArgumentException(
                "Course fee is outside the allowed range.");
    }
}
