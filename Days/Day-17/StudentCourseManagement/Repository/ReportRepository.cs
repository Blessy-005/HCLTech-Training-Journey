
using Microsoft.Data.SqlClient;
using System.Data;

namespace StudentCourseManagement.Repository;

public class ReportRepository
{
    private readonly string _connectionString =
        @"Server=.\SQLEXPRESS;Database=StudentCourseDB;Trusted_Connection=True;TrustServerCertificate=True;";

    public async Task<List<(string DepartmentName, int StudentCount)>>
        GetStudentCountByDepartmentAsync()
    {
        const string sql = """
            SELECT d.DepartmentName,
                   COUNT(s.StudentId) AS StudentCount
            FROM Departments AS d
            LEFT JOIN Students AS s
                ON d.DepartmentId = s.DepartmentId
            GROUP BY d.DepartmentId, d.DepartmentName
            ORDER BY d.DepartmentName;
            """;

        var reports = new List<(string DepartmentName, int StudentCount)>();

        await using SqlConnection connection = new(_connectionString);
        await using SqlCommand command = new(sql, connection);

        await connection.OpenAsync();

        await using SqlDataReader reader =
            await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            reports.Add((
                reader.GetString(0),
                reader.GetInt32(1)
            ));
        }

        return reports;
    }

    public async Task<List<(string CourseName, int EnrollmentCount)>>
        GetEnrollmentCountByCourseAsync()
    {
        const string sql = """
        SELECT c.CourseName,
               COUNT(e.EnrollmentId) AS EnrollmentCount
        FROM Courses AS c
        LEFT JOIN Enrollments AS e
            ON c.CourseId = e.CourseId
        GROUP BY c.CourseId, c.CourseName
        ORDER BY EnrollmentCount DESC;
        """;

        var reports = new List<(string CourseName, int EnrollmentCount)>();

        await using SqlConnection connection = new(_connectionString);
        await using SqlCommand command = new(sql, connection);

        await connection.OpenAsync();
        await using SqlDataReader reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            reports.Add((reader.GetString(0), reader.GetInt32(1)));
        }

        return reports;
    }

    public async Task<List<(int StudentId, string Name)>>
        GetStudentsWithoutEnrollmentsAsync()
    {
        const string sql = """
        SELECT s.StudentId, s.Name
        FROM Students AS s
        WHERE NOT EXISTS
        (
            SELECT 1
            FROM Enrollments AS e
            WHERE e.StudentId = s.StudentId
        )
        ORDER BY s.Name;
        """;

        var reports = new List<(int StudentId, string Name)>();

        await using SqlConnection connection = new(_connectionString);
        await using SqlCommand command = new(sql, connection);

        await connection.OpenAsync();
        await using SqlDataReader reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            reports.Add((reader.GetInt32(0), reader.GetString(1)));
        }

        return reports;
    }

    public async Task<List<(int CourseId, string CourseName)>>
        GetCoursesWithoutEnrollmentsAsync()
    {
        const string sql = """
        SELECT c.CourseId, c.CourseName
        FROM Courses AS c
        LEFT JOIN Enrollments AS e
            ON c.CourseId = e.CourseId
        WHERE e.EnrollmentId IS NULL
        ORDER BY c.CourseName;
        """;

        var reports = new List<(int CourseId, string CourseName)>();

        await using SqlConnection connection = new(_connectionString);
        await using SqlCommand command = new(sql, connection);

        await connection.OpenAsync();
        await using SqlDataReader reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            reports.Add((reader.GetInt32(0), reader.GetString(1)));
        }

        return reports;
    }

}
