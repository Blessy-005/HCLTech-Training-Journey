
using StudentCourseManagement.Models;
using StudentCourseManagement.Repositories;

namespace StudentCourseManagement.Services;

public class StudentService
{
    private readonly StudentRepository _repository;

    public StudentService(StudentRepository repository)
    {
        _repository = repository;
    }

    public Task<int> RegisterAsync(Student student)
    {
        if (string.IsNullOrWhiteSpace(student.Name))
            throw new ArgumentException("Student name is required.");

        if (string.IsNullOrWhiteSpace(student.Email))
            throw new ArgumentException("Student email is required.");

        if (!student.Email.Contains('@'))
            throw new ArgumentException("Enter a valid email address.");

        if (student.Age is < 17 or > 60)
            throw new ArgumentOutOfRangeException(
                nameof(student.Age),
                "Age must be between 17 and 60.");

        if (student.DepartmentId <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(student.DepartmentId),
                "Department ID must be positive.");

        return _repository.AddAsync(student);
    }

    public Task<List<Student>> GetAllAsync()
    {
        return _repository.GetAllAsync();
    }

    public Task<List<Student>> SearchByNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Search name cannot be empty.",
                nameof(name));
        }

        return _repository.SearchByNameAsync(name.Trim());
    }

    public Task<Student?> GetByIdAsync(int studentId)
    {
        if (studentId <= 0)
        {
            throw new ArgumentException(
                "Student ID must be greater than zero.",
                nameof(studentId));
        }

        return _repository.GetByIdAsync(studentId);
    }
    public Task<bool> UpdateAsync(Student student)
    {
        if (student.StudentId <= 0)
            throw new ArgumentException("Student ID must be greater than zero.");

        if (string.IsNullOrWhiteSpace(student.Name))
            throw new ArgumentException("Student name cannot be empty.");

        if (string.IsNullOrWhiteSpace(student.Email))
            throw new ArgumentException("Student email cannot be empty.");

        if (student.Age <= 0)
            throw new ArgumentException("Student age must be greater than zero.");

        if (student.DepartmentId <= 0)
            throw new ArgumentException("Department ID must be greater than zero.");

        if (student.AdmissionDate == default)
            throw new ArgumentException("Admission date is required.");

        student.Name = student.Name.Trim();
        student.Email = student.Email.Trim();

        return _repository.UpdateAsync(student);
    }

    public Task<bool> DeactivateAsync(int studentId)
    {
        if (studentId <= 0)
        {
            throw new ArgumentException(
                "Student ID must be greater than zero.",
                nameof(studentId));
        }

        return _repository.DeactivateAsync(studentId);
    }

    public Task<List<Student>> GetByDepartmentAsync(int departmentId)
    {
        if (departmentId <= 0)
        {
            throw new ArgumentException(
                "Department ID must be greater than zero.",
                nameof(departmentId));
        }

        return _repository.GetByDepartmentAsync(departmentId);
    }

}
