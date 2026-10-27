
using Microsoft.Data.SqlClient;
using StudentCourseManagement.Helpers;
using StudentCourseManagement.Models;
using System.Data;

namespace StudentCourseManagement.Repositories;

public class StudentRepository
{
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
            Student student = new()
            {
                StudentId = reader.GetInt32(reader.GetOrdinal("StudentId")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                Age = reader.GetInt32(reader.GetOrdinal("Age")),
                DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                AdmissionDate = reader.GetDateTime(reader.GetOrdinal("AdmissionDate")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
            };

            students.Add(student);
        }

        return students;
    }

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

        command.Parameters.Add("@StudentId", SqlDbType.Int).Value = studentId;

        await connection.OpenAsync();

        await using SqlDataReader reader =
            await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new Student
            {
                StudentId = reader.GetInt32(reader.GetOrdinal("StudentId")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                Age = reader.GetInt32(reader.GetOrdinal("Age")),
                DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                AdmissionDate = reader.GetDateTime(reader.GetOrdinal("AdmissionDate")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
            };
        }

        return null;
    }

    

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

        command.Parameters.Add("@Email", SqlDbType.NVarChar, 100)
            .Value = student.Email;

        command.Parameters.Add("@Age", SqlDbType.Int)
            .Value = student.Age;

        command.Parameters.Add("@DepartmentId", SqlDbType.Int)
            .Value = student.DepartmentId;

        command.Parameters.Add("@AdmissionDate", SqlDbType.Date)
            .Value = student.AdmissionDate;

        await connection.OpenAsync();

        int rowsAffected = await command.ExecuteNonQueryAsync();

        return rowsAffected > 0;
    }

    
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
        Student student = new()
        {
            StudentId = reader.GetInt32(reader.GetOrdinal("StudentId")),
            Name = reader.GetString(reader.GetOrdinal("Name")),
            Email = reader.GetString(reader.GetOrdinal("Email")),
            Age = reader.GetInt32(reader.GetOrdinal("Age")),
            DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
            AdmissionDate = reader.GetDateTime(reader.GetOrdinal("AdmissionDate")),
            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
        };

        students.Add(student);
    }

    return students;
}

}
