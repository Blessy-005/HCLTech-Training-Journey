
using Microsoft.Data.SqlClient;
using StudentCourseManagement.Helpers;
using StudentCourseManagement.Models;
using System.Data;

namespace StudentCourseManagement.Repository;

public class EnrollmentRepository
{
    // 1. Add an enrollment without an audit transaction
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

        return Convert.ToInt32(
            await command.ExecuteScalarAsync());
    }

    // 2. Add an enrollment and its audit log in one transaction

    public async Task<int> AddWithAuditAsync(Enrollment enrollment)
    {
        const string studentCheckSql = """
        SELECT COUNT(*)
        FROM Students
        WHERE StudentId = @StudentId
          AND IsActive = 1;
        """;

        const string courseCheckSql = """
        SELECT COUNT(*)
        FROM Courses
        WHERE CourseId = @CourseId
          AND IsActive = 1;
        """;

        const string enrollmentSql = """
        INSERT INTO Enrollments (StudentId, CourseId, Status)
        VALUES (@StudentId, @CourseId, @Status);

        SELECT CAST(SCOPE_IDENTITY() AS INT);
        """;

        const string auditSql = """
        INSERT INTO AuditLogs
            (Action, EntityName, EntityId, Details)
        VALUES
            (@Action, @EntityName, @EntityId, @Details);
        """;

        await using SqlConnection connection =
            DatabaseHelper.CreateConnection();

        await connection.OpenAsync();

        using SqlTransaction transaction =
            connection.BeginTransaction();

        try
        {
            // Check whether the student exists and is active.
            await using (SqlCommand command =
                new(studentCheckSql, connection, transaction))
            {
                command.Parameters.Add("@StudentId", SqlDbType.Int)
                    .Value = enrollment.StudentId;

                int studentCount = Convert.ToInt32(
                    await command.ExecuteScalarAsync());

                if (studentCount == 0)
                {
                    throw new InvalidOperationException(
                        "Student does not exist or is inactive.");
                }
            }

            // Check whether the course exists and is active.
            await using (SqlCommand command =
                new(courseCheckSql, connection, transaction))
            {
                command.Parameters.Add("@CourseId", SqlDbType.Int)
                    .Value = enrollment.CourseId;

                int courseCount = Convert.ToInt32(
                    await command.ExecuteScalarAsync());

                if (courseCount == 0)
                {
                    throw new InvalidOperationException(
                        "Course does not exist or is inactive.");
                }
            }

            // Insert the enrollment only after both checks pass.
            int enrollmentId;

            await using (SqlCommand command =
                new(enrollmentSql, connection, transaction))
            {
                command.Parameters.Add("@StudentId", SqlDbType.Int)
                    .Value = enrollment.StudentId;

                command.Parameters.Add("@CourseId", SqlDbType.Int)
                    .Value = enrollment.CourseId;

                command.Parameters.Add("@Status", SqlDbType.NVarChar, 20)
                    .Value = enrollment.Status;

                enrollmentId = Convert.ToInt32(
                    await command.ExecuteScalarAsync());
            }

            // Record the audit log in the same transaction.
            await using (SqlCommand command =
                new(auditSql, connection, transaction))
            {
                command.Parameters.Add("@Action", SqlDbType.NVarChar, 20)
                    .Value = "CREATE";

                command.Parameters.Add("@EntityName", SqlDbType.NVarChar, 50)
                    .Value = "Enrollment";

                command.Parameters.Add("@EntityId", SqlDbType.Int)
                    .Value = enrollmentId;

                command.Parameters.Add("@Details", SqlDbType.NVarChar, 500)
                    .Value =
                        $"StudentId: {enrollment.StudentId}, " +
                        $"CourseId: {enrollment.CourseId}, " +
                        $"Status: {enrollment.Status}";

                await command.ExecuteNonQueryAsync();
            }

            transaction.Commit();

            return enrollmentId;
        }
        catch
        {
            try
            {
                transaction.Rollback();
            }
            catch
            {
                // Preserve the original exception.
            }

            throw;
        }
    }


    // 3. Get all enrollments
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

    // 4. Get an enrollment by ID
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

    // 5. Update enrollment status
    public async Task<bool> UpdateStatusAsync(
     int enrollmentId, string status)
    {
        const string updateSql = """
        UPDATE Enrollments
        SET Status = @Status
        WHERE EnrollmentId = @EnrollmentId;
        """;

        const string auditSql = """
        INSERT INTO AuditLogs
            (EntityName, EntityId, Action, Details)
        VALUES
            (@EntityName, @EntityId, @Action, @Details);
        """;

        await using SqlConnection connection =
            DatabaseHelper.CreateConnection();

        await connection.OpenAsync();

        using SqlTransaction transaction =
            connection.BeginTransaction();

        try
        {
            int rowsAffected;

            await using (SqlCommand command =
                new(updateSql, connection, transaction))
            {
                command.Parameters.Add("@EnrollmentId", SqlDbType.Int)
                    .Value = enrollmentId;

                command.Parameters.Add("@Status", SqlDbType.NVarChar, 20)
                    .Value = status;

                rowsAffected = await command.ExecuteNonQueryAsync();
            }

            // If no enrollment exists, don't create an audit record.
            if (rowsAffected == 0)
            {
                transaction.Rollback();
                return false;
            }

            await using (SqlCommand command =
                new(auditSql, connection, transaction))
            {
                command.Parameters.Add("@EntityName", SqlDbType.NVarChar, 50)
                    .Value = "Enrollment";

                command.Parameters.Add("@EntityId", SqlDbType.Int)
                    .Value = enrollmentId;

                command.Parameters.Add("@Action", SqlDbType.NVarChar, 20)
                    .Value = status == "Completed"
                        ? "COMPLETE"
                        : "CANCEL";

                command.Parameters.Add("@Details", SqlDbType.NVarChar, 500)
                    .Value = $"Enrollment {enrollmentId} status changed to {status}.";

                await command.ExecuteNonQueryAsync();
            }

            transaction.Commit();
            return true;
        }
        catch
        {
            try
            {
                transaction.Rollback();
            }
            catch
            {
                // Preserve the original exception.
            }

            throw;
        }
    }
}
