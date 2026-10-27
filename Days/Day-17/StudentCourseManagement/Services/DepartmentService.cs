
using StudentCourseManagement.Models;
using StudentCourseManagement.Repositories;

namespace StudentCourseManagement.Services;

public class DepartmentService
{
    private readonly DepartmentRepository _repository;

    public DepartmentService(DepartmentRepository repository)
    {
        _repository = repository;
    }

    // Add a department
    public async Task<int> AddAsync(Department department)
    {
        ValidateDepartment(department);

        department.DepartmentName = department.DepartmentName.Trim();
        department.Location = department.Location?.Trim();

        return await _repository.AddAsync(department);
    }

    // List all departments
    public Task<List<Department>> GetAllAsync()
    {
        return _repository.GetAllAsync();
    }

    // Search department by ID
    public async Task<Department?> GetByIdAsync(int departmentId)
    {
        if (departmentId <= 0)
        {
            throw new ArgumentException(
                "Department ID must be greater than zero.",
                nameof(departmentId));
        }

        return await _repository.GetByIdAsync(departmentId);
    }

    // Update a department
    public async Task<bool> UpdateAsync(Department department)
    {
        if (department == null)
        {
            throw new ArgumentNullException(nameof(department));
        }

        if (department.DepartmentId <= 0)
        {
            throw new ArgumentException(
                "Department ID must be greater than zero.",
                nameof(department.DepartmentId));
        }

        ValidateDepartment(department);

        department.DepartmentName = department.DepartmentName.Trim();
        department.Location = department.Location?.Trim();

        return await _repository.UpdateAsync(department);
    }

    // Deactivate a department
    public async Task<bool> DeactivateAsync(int departmentId)
    {
        if (departmentId <= 0)
        {
            throw new ArgumentException(
                "Department ID must be greater than zero.",
                nameof(departmentId));
        }

        return await _repository.DeactivateAsync(departmentId);
    }

    // Validate department details
    private static void ValidateDepartment(Department department)
    {
        if (department == null)
        {
            throw new ArgumentNullException(nameof(department));
        }

        if (string.IsNullOrWhiteSpace(department.DepartmentName))
        {
            throw new ArgumentException(
                "Department name is required.",
                nameof(department.DepartmentName));
        }

        if (department.DepartmentName.Trim().Length > 100)
        {
            throw new ArgumentException(
                "Department name cannot exceed 100 characters.",
                nameof(department.DepartmentName));
        }

        if (department.Location?.Trim().Length > 100)
        {
            throw new ArgumentException(
                "Department location cannot exceed 100 characters.",
                nameof(department.Location));
        }
    }
}
