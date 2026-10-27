
using Microsoft.Data.SqlClient;
using StudentCourseManagement.Helpers;
using StudentCourseManagement.Models;
using System.Data;

namespace StudentCourseManagement.Repositories;

public class DepartmentRepository
{
    // Add a new department
    public async Task<int> AddAsync(Department department)
    {
        const string sql = """
            INSERT INTO Departments (DepartmentName, Location)
            VALUES (@DepartmentName, @Location);

            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;

        await using SqlConnection connection =
            DatabaseHelper.CreateConnection();

        await using SqlCommand command = new(sql, connection);

        command.Parameters.Add("@DepartmentName", SqlDbType.NVarChar, 100)
            .Value = department.DepartmentName;

        command.Parameters.Add("@Location", SqlDbType.NVarChar, 100)
            .Value = (object?)department.Location ?? DBNull.Value;

        await connection.OpenAsync();

        object? result = await command.ExecuteScalarAsync();

        return Convert.ToInt32(result);
    }

    // Get all departments
    public async Task<List<Department>> GetAllAsync()
    {
        const string sql = """
            SELECT DepartmentId, DepartmentName, Location, IsActive
            FROM Departments
            ORDER BY DepartmentId;
            """;

        List<Department> departments = new();

        await using SqlConnection connection =
            DatabaseHelper.CreateConnection();

        await using SqlCommand command = new(sql, connection);

        await connection.OpenAsync();

        await using SqlDataReader reader =
            await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            departments.Add(ReadDepartment(reader));
        }

        return departments;
    }

    // Get a department by ID
    public async Task<Department?> GetByIdAsync(int departmentId)
    {
        const string sql = """
            SELECT DepartmentId, DepartmentName, Location, IsActive
            FROM Departments
            WHERE DepartmentId = @DepartmentId;
            """;

        await using SqlConnection connection =
            DatabaseHelper.CreateConnection();

        await using SqlCommand command = new(sql, connection);

        command.Parameters.Add("@DepartmentId", SqlDbType.Int)
            .Value = departmentId;

        await connection.OpenAsync();

        await using SqlDataReader reader =
            await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return ReadDepartment(reader);
        }

        return null;
    }

    // Update a department
    public async Task<bool> UpdateAsync(Department department)
    {
        const string sql = """
            UPDATE Departments
            SET DepartmentName = @DepartmentName,
                Location = @Location
            WHERE DepartmentId = @DepartmentId
              AND IsActive = 1;
            """;

        await using SqlConnection connection =
            DatabaseHelper.CreateConnection();

        await using SqlCommand command = new(sql, connection);

        command.Parameters.Add("@DepartmentId", SqlDbType.Int)
            .Value = department.DepartmentId;

        command.Parameters.Add("@DepartmentName", SqlDbType.NVarChar, 100)
            .Value = department.DepartmentName;

        command.Parameters.Add("@Location", SqlDbType.NVarChar, 100)
            .Value = (object?)department.Location ?? DBNull.Value;

        await connection.OpenAsync();

        int rowsAffected = await command.ExecuteNonQueryAsync();

        return rowsAffected > 0;
    }

    // Deactivate a department
    public async Task<bool> DeactivateAsync(int departmentId)
    {
        const string sql = """
            UPDATE Departments
            SET IsActive = 0
            WHERE DepartmentId = @DepartmentId
              AND IsActive = 1;
            """;

        await using SqlConnection connection =
            DatabaseHelper.CreateConnection();

        await using SqlCommand command = new(sql, connection);

        command.Parameters.Add("@DepartmentId", SqlDbType.Int)
            .Value = departmentId;

        await connection.OpenAsync();

        int rowsAffected = await command.ExecuteNonQueryAsync();

        return rowsAffected > 0;
    }

    // Check whether a department exists and is active
    public async Task<bool> IsDepartmentActiveAsync(int departmentId)
    {
        const string sql = """
            SELECT COUNT(1)
            FROM Departments
            WHERE DepartmentId = @DepartmentId
              AND IsActive = 1;
            """;

        await using SqlConnection connection =
            DatabaseHelper.CreateConnection();

        await using SqlCommand command = new(sql, connection);

        command.Parameters.Add("@DepartmentId", SqlDbType.Int)
            .Value = departmentId;

        await connection.OpenAsync();

        int count = Convert.ToInt32(
            await command.ExecuteScalarAsync());

        return count > 0;
    }

    // Convert a database row into a Department object
    private static Department ReadDepartment(SqlDataReader reader)
    {
        return new Department
        {
            DepartmentId = reader.GetInt32(
                reader.GetOrdinal("DepartmentId")),

            DepartmentName = reader.GetString(
                reader.GetOrdinal("DepartmentName")),

            Location = reader.IsDBNull(
                reader.GetOrdinal("Location"))
                ? null
                : reader.GetString(reader.GetOrdinal("Location")),

            IsActive = reader.GetBoolean(
                reader.GetOrdinal("IsActive"))
        };
    }
}
