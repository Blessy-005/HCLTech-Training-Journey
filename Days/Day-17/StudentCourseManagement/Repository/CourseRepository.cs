
using Microsoft.Data.SqlClient;
using System.Data;
using StudentCourseManagement.Models;

namespace StudentCourseManagement.Repository;

public class CourseRepository
{
    private readonly string _connectionString;

    public CourseRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    // 1. Add a course
    public async Task AddCourseAsync(Course course)
    {
        const string query = """
            INSERT INTO Courses
                (CourseName, DurationMonths, Fee, DepartmentId)
            VALUES
                (@CourseName, @DurationMonths, @Fee, @DepartmentId)
            """;

        await using var connection = new SqlConnection(_connectionString);
        await using var command = new SqlCommand(query, connection);

        command.Parameters.Add("@CourseName", SqlDbType.NVarChar, 100)
            .Value = course.CourseName;

        command.Parameters.Add("@DurationMonths", SqlDbType.Int)
            .Value = course.DurationMonths;

        command.Parameters.Add("@Fee", SqlDbType.Decimal)
            .Value = course.Fee;
        command.Parameters["@Fee"].Precision = 10;
        command.Parameters["@Fee"].Scale = 2;

        command.Parameters.Add("@DepartmentId", SqlDbType.Int)
            .Value = course.DepartmentId;

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    // 2. View all courses
    public async Task<List<Course>> GetAllCoursesAsync()
    {
        const string query = """
            SELECT CourseId, CourseName, DurationMonths,
                   Fee, DepartmentId, IsActive
            FROM Courses
            ORDER BY CourseId
            """;

        var courses = new List<Course>();

        await using var connection = new SqlConnection(_connectionString);
        await using var command = new SqlCommand(query, connection);

        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            courses.Add(MapCourse(reader));
        }

        return courses;
    }

    // 3. Search by course ID
    public async Task<Course?> GetCourseByIdAsync(int id)
    {
        const string query = """
            SELECT CourseId, CourseName, DurationMonths,
                   Fee, DepartmentId, IsActive
            FROM Courses
            WHERE CourseId = @CourseId
            """;

        await using var connection = new SqlConnection(_connectionString);
        await using var command = new SqlCommand(query, connection);

        command.Parameters.Add("@CourseId", SqlDbType.Int).Value = id;

        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
            return MapCourse(reader);

        return null;
    }

    // 4. Filter courses by department
    public async Task<List<Course>> GetCoursesByDepartmentAsync(
        int departmentId)
    {
        const string query = """
            SELECT CourseId, CourseName, DurationMonths,
                   Fee, DepartmentId, IsActive
            FROM Courses
            WHERE DepartmentId = @DepartmentId
            ORDER BY CourseName
            """;

        var courses = new List<Course>();

        await using var connection = new SqlConnection(_connectionString);
        await using var command = new SqlCommand(query, connection);

        command.Parameters.Add("@DepartmentId", SqlDbType.Int)
            .Value = departmentId;

        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
            courses.Add(MapCourse(reader));

        return courses;
    }

    // 5. Update a course
    public async Task<bool> UpdateCourseAsync(Course course)
    {
        const string query = """
            UPDATE Courses
            SET CourseName = @CourseName,
                DurationMonths = @DurationMonths,
                Fee = @Fee,
                DepartmentId = @DepartmentId
            WHERE CourseId = @CourseId
              AND IsActive = 1
            """;

        await using var connection = new SqlConnection(_connectionString);
        await using var command = new SqlCommand(query, connection);

        command.Parameters.Add("@CourseId", SqlDbType.Int)
            .Value = course.CourseId;

        command.Parameters.Add("@CourseName", SqlDbType.NVarChar, 100)
            .Value = course.CourseName;

        command.Parameters.Add("@DurationMonths", SqlDbType.Int)
            .Value = course.DurationMonths;

        var feeParameter = command.Parameters.Add("@Fee", SqlDbType.Decimal);
        feeParameter.Precision = 10;
        feeParameter.Scale = 2;
        feeParameter.Value = course.Fee;

        command.Parameters.Add("@DepartmentId", SqlDbType.Int)
            .Value = course.DepartmentId;

        await connection.OpenAsync();

        return await command.ExecuteNonQueryAsync() > 0;
    }

    // 6. Deactivate a course without deleting it
    public async Task<bool> DeactivateCourseAsync(int id)
    {
        const string query = """
            UPDATE Courses
            SET IsActive = 0
            WHERE CourseId = @CourseId
              AND IsActive = 1
            """;

        await using var connection = new SqlConnection(_connectionString);
        await using var command = new SqlCommand(query, connection);

        command.Parameters.Add("@CourseId", SqlDbType.Int).Value = id;

        await connection.OpenAsync();

        return await command.ExecuteNonQueryAsync() > 0;
    }

    // Common method to convert a database row into a Course object
    private static Course MapCourse(SqlDataReader reader)
    {
        return new Course
        {
            CourseId = reader.GetInt32(0),
            CourseName = reader.GetString(1),
            DurationMonths = reader.GetInt32(2),
            Fee = reader.GetDecimal(3),
            DepartmentId = reader.GetInt32(4),
            IsActive = reader.GetBoolean(5)
        };
    }
}
