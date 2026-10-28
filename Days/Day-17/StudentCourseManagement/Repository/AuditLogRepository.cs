
using Microsoft.Data.SqlClient;
using StudentCourseManagement.Models;

namespace StudentCourseManagement.Repository;

public class AuditLogRepository
{
    private readonly string _connectionString =
        @"Server=.\SQLEXPRESS;Database=StudentCourseDB;Trusted_Connection=True;TrustServerCertificate=True;";

    // Add a new audit log
    public async Task AddAsync(AuditLog auditLog)
    {
        const string sql = """
            INSERT INTO AuditLogs
                (Action, EntityName, EntityId, Details)
            VALUES
                (@Action, @EntityName, @EntityId, @Details);
            """;

        await using SqlConnection connection =
            new(_connectionString);

        await using SqlCommand command =
            new(sql, connection);

        command.Parameters.AddWithValue(
            "@Action", auditLog.Action);

        command.Parameters.AddWithValue(
            "@EntityName", auditLog.EntityName);

        command.Parameters.AddWithValue(
            "@EntityId",
            (object?)auditLog.EntityId ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@Details",
            string.IsNullOrWhiteSpace(auditLog.Details)
                ? DBNull.Value
                : auditLog.Details);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    // Get the latest audit logs
    public async Task<List<AuditLog>> GetRecentAsync(int count = 20)
    {
        const string sql = """
            SELECT TOP (@Count)
                AuditId,
                Action,
                EntityName,
                EntityId,
                Details,
                CreatedAt
            FROM AuditLogs
            ORDER BY CreatedAt DESC, AuditId DESC;
            """;

        var logs = new List<AuditLog>();

        await using SqlConnection connection =
            new(_connectionString);

        await using SqlCommand command =
            new(sql, connection);

        command.Parameters.AddWithValue(
            "@Count", Math.Max(1, count));

        await connection.OpenAsync();

        await using SqlDataReader reader =
            await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            logs.Add(new AuditLog
            {
                AuditId = Convert.ToInt32(reader.GetInt64(0)),
                Action = reader.GetString(1),
                EntityName = reader.GetString(2),
                EntityId = reader.IsDBNull(3)
                    ? null
                    : reader.GetInt32(3),
                Details = reader.IsDBNull(4)
                    ? string.Empty
                    : reader.GetString(4),
                CreatedAt = reader.GetDateTime(5)
            });
        }

        return logs;
    }
}
