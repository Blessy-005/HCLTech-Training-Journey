
using Microsoft.Data.SqlClient;
using StudentCourseManagement.Helpers;
using StudentCourseManagement.Models;
using System.Data;

namespace StudentCourseManagement.Repositories;

public class StudentRepository
{
    // Register a new student
    public async Task<int> AddAsync(Student student)
    {
        const string sql = """
            INSERT INTO Students (Name, Email, Age, DepartmentId)
            VALUES (@Name, @Email, @Age, @DepartmentId);

            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;

        await using SqlConnection connection =
            DatabaseHelper.CreateConnection();

        await using SqlCommand command = new(sql, connection);

        command.Parameters.Add("@Name", SqlDbType.NVarChar, 100)
            .Value = student.Name;

        command.Parameters.Add("@Email", SqlDbType.NVarChar, 255)
            .Value = student.Email;

        command.Parameters.Add("@Age", SqlDbType.Int)
            .Value = student.Age;

        command.Parameters.Add("@DepartmentId", SqlDbType.Int)
            .Value = student.DepartmentId;

        await connection.OpenAsync();

        object? result = await command.ExecuteScalarAsync();

        return Convert.ToInt32(result);
    }

    // Get all active students
    public async Task<List<Student>> GetAllAsync()
    {
        const string sql = """
            SELECT StudentId, Name, Email, Age, DepartmentId,
                   AdmissionDate, IsActive
            FROM Students
            WHERE IsActive = 1
            ORDER BY StudentId;
            """;

        List<Student> students = new();

        await using SqlConnection connection =
            DatabaseHelper.CreateConnection();

        await using SqlCommand command = new(sql, connection);

        await connection.OpenAsync();

        await using SqlDataReader reader =
            await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            students.Add(ReadStudent(reader));
        }

        return students;
    }

    // Get an active student by ID
    public async Task<Student?> GetByIdAsync(int studentId)
    {
        const string sql = """
            SELECT StudentId, Name, Email, Age, DepartmentId,
                   AdmissionDate, IsActive
            FROM Students
            WHERE StudentId = @StudentId
              AND IsActive = 1;
            """;

        await using SqlConnection connection =
            DatabaseHelper.CreateConnection();

        await using SqlCommand command = new(sql, connection);

        command.Parameters.Add("@StudentId", SqlDbType.Int)
            .Value = studentId;

        await connection.OpenAsync();

        await using SqlDataReader reader =
            await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return ReadStudent(reader);
        }

        return null;
    }

    // Update an active student
    public async Task<bool> UpdateAsync(Student student)
    {
        const string sql = """
            UPDATE Students
            SET Name = @Name,
                Email = @Email,
                Age = @Age,
                DepartmentId = @DepartmentId,
                AdmissionDate = @AdmissionDate
            WHERE StudentId = @StudentId
              AND IsActive = 1;
            """;

        await using SqlConnection connection =
            DatabaseHelper.CreateConnection();

        await using SqlCommand command = new(sql, connection);

        command.Parameters.Add("@StudentId", SqlDbType.Int)
            .Value = student.StudentId;

        command.Parameters.Add("@Name", SqlDbType.NVarChar, 100)
            .Value = student.Name;

        command.Parameters.Add("@Email", SqlDbType.NVarChar, 255)
            .Value = student.Email;

        command.Parameters.Add("@Age", SqlDbType.Int)
            .Value = student.Age;

        command.Parameters.Add("@DepartmentId", SqlDbType.Int)
            .Value = student.DepartmentId;

        command.Parameters.Add("@AdmissionDate", SqlDbType.DateTime2)
            .Value = student.AdmissionDate;

        await connection.OpenAsync();

        int rowsAffected = await command.ExecuteNonQueryAsync();

        return rowsAffected > 0;
    }

    // Search active students by partial name
    public async Task<List<Student>> SearchByNameAsync(string name)
    {
        const string sql = """
            SELECT StudentId, Name, Email, Age, DepartmentId,
                   AdmissionDate, IsActive
            FROM Students
            WHERE Name LIKE @Name
              AND IsActive = 1
            ORDER BY Name;
            """;

        List<Student> students = new();

        await using SqlConnection connection =
            DatabaseHelper.CreateConnection();

        await using SqlCommand command = new(sql, connection);

        command.Parameters.Add("@Name", SqlDbType.NVarChar, 100)
            .Value = $"%{name}%";

        await connection.OpenAsync();

        await using SqlDataReader reader =
            await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            students.Add(ReadStudent(reader));
        }

        return students;
    }

    // Deactivate an active student
    public async Task<bool> DeactivateAsync(int studentId)
    {
        const string sql = """
            UPDATE Students
            SET IsActive = 0
            WHERE StudentId = @StudentId
              AND IsActive = 1;
            """;

        await using SqlConnection connection =
            DatabaseHelper.CreateConnection();

        await using SqlCommand command = new(sql, connection);

        command.Parameters.Add("@StudentId", SqlDbType.Int)
            .Value = studentId;

        await connection.OpenAsync();

        int rowsAffected = await command.ExecuteNonQueryAsync();

        return rowsAffected > 0;
    }

    // Get active students belonging to a department
    public async Task<List<Student>> GetByDepartmentAsync(int departmentId)
    {
        const string sql = """
            SELECT StudentId, Name, Email, Age, DepartmentId,
                   AdmissionDate, IsActive
            FROM Students
            WHERE DepartmentId = @DepartmentId
              AND IsActive = 1
            ORDER BY Name;
            """;

        List<Student> students = new();

        await using SqlConnection connection =
            DatabaseHelper.CreateConnection();

        await using SqlCommand command = new(sql, connection);

        command.Parameters.Add("@DepartmentId", SqlDbType.Int)
            .Value = departmentId;

        await connection.OpenAsync();

        await using SqlDataReader reader =
            await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            students.Add(ReadStudent(reader));
        }

        return students;
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

    // Convert a database row into a Student object
    private static Student ReadStudent(SqlDataReader reader)
    {
        return new Student
        {
            StudentId = reader.GetInt32(
                reader.GetOrdinal("StudentId")),

            Name = reader.GetString(
                reader.GetOrdinal("Name")),

            Email = reader.GetString(
                reader.GetOrdinal("Email")),

            Age = reader.GetInt32(
                reader.GetOrdinal("Age")),

            DepartmentId = reader.GetInt32(
                reader.GetOrdinal("DepartmentId")),

            AdmissionDate = reader.GetDateTime(
                reader.GetOrdinal("AdmissionDate")),

            IsActive = reader.GetBoolean(
                reader.GetOrdinal("IsActive"))
        };
    }
}
