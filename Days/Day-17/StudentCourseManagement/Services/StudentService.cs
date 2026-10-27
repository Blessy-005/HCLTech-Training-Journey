
using System.Net.Mail;
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

    // Register a new student
    public async Task<int> RegisterAsync(Student student)
    {
        ValidateStudentDetails(student);

        bool departmentExists =
            await _repository.IsDepartmentActiveAsync(student.DepartmentId);

        if (!departmentExists)
        {
            throw new ArgumentException(
                "Department does not exist or is inactive.");
        }

        return await _repository.AddAsync(student);
    }

    // Get all active students
    public Task<List<Student>> GetAllAsync()
    {
        return _repository.GetAllAsync();
    }

    // Search active students by partial name
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

    // Search an active student by ID
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

    // Update an existing active student
    public async Task<bool> UpdateAsync(Student student)
    {
        if (student == null)
        {
            throw new ArgumentNullException(nameof(student));
        }

        if (student.StudentId <= 0)
        {
            throw new ArgumentException(
                "Student ID must be greater than zero.",
                nameof(student.StudentId));
        }

        ValidateStudentDetails(student);

        if (student.AdmissionDate == default)
        {
            throw new ArgumentException(
                "Admission date is required.",
                nameof(student.AdmissionDate));
        }

        bool departmentExists =
            await _repository.IsDepartmentActiveAsync(student.DepartmentId);

        if (!departmentExists)
        {
            throw new ArgumentException(
                "Department does not exist or is inactive.");
        }

        return await _repository.UpdateAsync(student);
    }

    // Deactivate an active student
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

    // Get active students by department
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

    // Validate common student details
    private static void ValidateStudentDetails(Student student)
    {
        if (student == null)
        {
            throw new ArgumentNullException(nameof(student));
        }

        if (string.IsNullOrWhiteSpace(student.Name))
        {
            throw new ArgumentException(
                "Student name is required.",
                nameof(student.Name));
        }

        if (string.IsNullOrWhiteSpace(student.Email))
        {
            throw new ArgumentException(
                "Student email is required.",
                nameof(student.Email));
        }

        student.Name = student.Name.Trim();
        student.Email = student.Email.Trim();

        try
        {
            MailAddress parsedEmail = new(student.Email);

            if (parsedEmail.Address != student.Email)
            {
                throw new ArgumentException(
                    "Enter a valid email address.",
                    nameof(student.Email));
            }
        }
        catch (FormatException)
        {
            throw new ArgumentException(
                "Enter a valid email address.",
                nameof(student.Email));
        }

        if (student.Age is < 17 or > 60)
        {
            throw new ArgumentOutOfRangeException(
                nameof(student.Age),
                "Age must be between 17 and 60.");
        }

        if (student.DepartmentId <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(student.DepartmentId),
                "Department ID must be positive.");
        }
    }
}
