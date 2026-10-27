
using Microsoft.Data.SqlClient;
using System.Data;
using StudentCourseManagement.Helpers;
using StudentCourseManagement.Models;

namespace StudentCourseManagement.Repository;

public class EnrollmentRepository
{
    public async Task<int> AddAsync(Enrollment enrollment)
    {
        const string sql = """
            INSERT INTO Enrollments (StudentId, CourseId, Status)
            VALUES (@StudentId, @CourseId, @Status);
            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;

        await using SqlConnection connection =
            DatabaseHelper.CreateConnection();

        await using SqlCommand command = new(sql, connection);

        command.Parameters.Add("@StudentId", SqlDbType.Int)
            .Value = enrollment.StudentId;

        command.Parameters.Add("@CourseId", SqlDbType.Int)
            .Value = enrollment.CourseId;

        command.Parameters.Add("@Status", SqlDbType.NVarChar, 20)
            .Value = enrollment.Status;

        await connection.OpenAsync();

        return Convert.ToInt32(await command.ExecuteScalarAsync());
    }

    public async Task<List<Enrollment>> GetAllAsync()
    {
        const string sql = """
            SELECT EnrollmentId, StudentId, CourseId, EnrolledAt, Status
            FROM Enrollments
            ORDER BY EnrollmentId DESC;
            """;

        var enrollments = new List<Enrollment>();

        await using SqlConnection connection =
            DatabaseHelper.CreateConnection();

        await using SqlCommand command = new(sql, connection);

        await connection.OpenAsync();

        await using SqlDataReader reader =
            await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            enrollments.Add(new Enrollment
            {
                EnrollmentId = reader.GetInt32(0),
                StudentId = reader.GetInt32(1),
                CourseId = reader.GetInt32(2),
                EnrolledAt = reader.GetDateTime(3),
                Status = reader.GetString(4)
            });
        }

        return enrollments;
    }

    public async Task<Enrollment?> GetByIdAsync(int enrollmentId)
    {
        const string sql = """
            SELECT EnrollmentId, StudentId, CourseId, EnrolledAt, Status
            FROM Enrollments
            WHERE EnrollmentId = @EnrollmentId;
            """;

        await using SqlConnection connection =
            DatabaseHelper.CreateConnection();

        await using SqlCommand command = new(sql, connection);

        command.Parameters.Add("@EnrollmentId", SqlDbType.Int)
            .Value = enrollmentId;

        await connection.OpenAsync();

        await using SqlDataReader reader =
            await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        return new Enrollment
        {
            EnrollmentId = reader.GetInt32(0),
            StudentId = reader.GetInt32(1),
            CourseId = reader.GetInt32(2),
            EnrolledAt = reader.GetDateTime(3),
            Status = reader.GetString(4)
        };
    }

    public async Task<bool> UpdateStatusAsync(
        int enrollmentId, string status)
    {
        const string sql = """
            UPDATE Enrollments
            SET Status = @Status
            WHERE EnrollmentId = @EnrollmentId;
            """;

        await using SqlConnection connection =
            DatabaseHelper.CreateConnection();

        await using SqlCommand command = new(sql, connection);

        command.Parameters.Add("@EnrollmentId", SqlDbType.Int)
            .Value = enrollmentId;

        command.Parameters.Add("@Status", SqlDbType.NVarChar, 20)
            .Value = status;

        await connection.OpenAsync();

        return await command.ExecuteNonQueryAsync() > 0;
    }
}
