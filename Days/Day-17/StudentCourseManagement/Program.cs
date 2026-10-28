using Microsoft.Data.SqlClient;
using StudentCourseManagement.Models;
using StudentCourseManagement.Repositories;
using StudentCourseManagement.Repository;
using StudentCourseManagement.Services;
using StudentCourseManagement.Repository;
using StudentCourseManagement.Services;
StudentRepository studentRepository = new();
StudentService studentService = new(studentRepository);

DepartmentRepository departmentRepository = new();
DepartmentService departmentService = new(departmentRepository);

string connectionString =
    @"Server=.\SQLEXPRESS;Database=StudentCourseDB;Trusted_Connection=True;TrustServerCertificate=True;";

CourseRepository courseRepository = new(connectionString);
CourseService courseService = new(courseRepository);
EnrollmentRepository enrollmentRepository = new();
EnrollmentService enrollmentService = new(enrollmentRepository);
var reportRepository = new ReportRepository();
var reportService = new ReportService(reportRepository);
var auditLogRepository = new AuditLogRepository();
var auditLogService = new AuditLogService(auditLogRepository);
bool running = true;

while (running)
{
    Console.WriteLine();
    Console.WriteLine("================================");
    Console.WriteLine(" MAIN MENU");
    Console.WriteLine("================================");
    Console.WriteLine("1. Student Management");
    Console.WriteLine("2. Department Management");
    Console.WriteLine("3. Course Management");
    Console.WriteLine("4. Enrollment Management");
    Console.WriteLine("5. Reports");
    Console.WriteLine("6. View Audit Logs");
    Console.WriteLine("0. Exit");
    Console.Write("Enter your choice: ");

    string? choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            await StudentMenu(studentService);
            break;

        case "2":
            await DepartmentMenu(departmentService);
            break;

        case "3":
            await CourseMenu(courseService);
            break;
        case "4":
            await EnrollmentMenu(enrollmentService);
            break;
        case "5":
            await ReportMenu(reportService);
            break;
        case "6":
            await ViewAuditLogs(auditLogService);
            break;
        case "0":
            running = false;
            Console.WriteLine("Application closed.");
            break;

        default:
            Console.WriteLine("Invalid choice. Try again.");
            break;
    }
}

static async Task StudentMenu(StudentService service)
{
    bool running = true;

    while (running)
    {
        Console.WriteLine();
        Console.WriteLine("================================");
        Console.WriteLine(" STUDENT MANAGEMENT");
        Console.WriteLine("================================");
        Console.WriteLine("1. Register Student");
        Console.WriteLine("2. List All Active Students");
        Console.WriteLine("3. Search Student by ID");
        Console.WriteLine("4. Search Student by Name");
        Console.WriteLine("5. Update Student");
        Console.WriteLine("6. Deactivate Student");
        Console.WriteLine("7. Filter Students by Department");
        Console.WriteLine("0. Back to Main Menu");
        Console.Write("Enter your choice: ");

        string? choice = Console.ReadLine();

        try
        {
            switch (choice)
            {
                case "1":
                    {
                        Console.Write("Enter student name: ");
                        string name = Console.ReadLine() ?? "";

                        Console.Write("Enter student email: ");
                        string email = Console.ReadLine() ?? "";

                        Console.Write("Enter student age: ");
                        if (!int.TryParse(Console.ReadLine(), out int age))
                        {
                            Console.WriteLine("Age must be a number.");
                            break;
                        }

                        Console.Write("Enter department ID: ");
                        if (!int.TryParse(Console.ReadLine(), out int departmentId))
                        {
                            Console.WriteLine("Department ID must be a number.");
                            break;
                        }

                        Student student = new()
                        {
                            Name = name,
                            Email = email,
                            Age = age,
                            DepartmentId = departmentId
                        };

                        int studentId = await service.RegisterAsync(student);

                        Console.WriteLine(
                            $"Student registered successfully! Student ID: {studentId}");
                        break;
                    }

                case "2":
                    {
                        List<Student> students = await service.GetAllAsync();

                        if (students.Count == 0)
                        {
                            Console.WriteLine("No active students found.");
                            break;
                        }

                        foreach (Student student in students)
                        {
                            PrintStudent(student);
                        }

                        break;
                    }

                case "3":
                    {
                        Console.Write("Enter student ID: ");

                        if (!int.TryParse(Console.ReadLine(), out int studentId))
                        {
                            Console.WriteLine("Student ID must be a number.");
                            break;
                        }

                        Student? student = await service.GetByIdAsync(studentId);

                        if (student == null)
                        {
                            Console.WriteLine("No active student found with that ID.");
                        }
                        else
                        {
                            PrintStudent(student);
                        }

                        break;
                    }

                case "4":
                    {
                        Console.Write("Enter student name to search: ");
                        string name = Console.ReadLine() ?? "";

                        List<Student> students =
                            await service.SearchByNameAsync(name);

                        if (students.Count == 0)
                        {
                            Console.WriteLine("No active students found.");
                            break;
                        }

                        foreach (Student student in students)
                        {
                            PrintStudent(student);
                        }

                        break;
                    }

                case "5":
                    {
                        Console.Write("Enter Student ID to update: ");

                        if (!int.TryParse(Console.ReadLine(), out int studentId) ||
                            studentId <= 0)
                        {
                            Console.WriteLine("Invalid student ID.");
                            break;
                        }

                        Student? student = await service.GetByIdAsync(studentId);

                        if (student == null)
                        {
                            Console.WriteLine("Active student not found.");
                            break;
                        }

                        Console.Write("Enter new name: ");
                        student.Name = Console.ReadLine() ?? "";

                        Console.Write("Enter new email: ");
                        student.Email = Console.ReadLine() ?? "";

                        Console.Write("Enter new age: ");
                        if (!int.TryParse(Console.ReadLine(), out int age))
                        {
                            Console.WriteLine("Invalid age.");
                            break;
                        }

                        student.Age = age;

                        Console.Write("Enter new department ID: ");
                        if (!int.TryParse(Console.ReadLine(), out int departmentId))
                        {
                            Console.WriteLine("Invalid department ID.");
                            break;
                        }

                        student.DepartmentId = departmentId;

                        Console.Write("Enter admission date (yyyy-MM-dd): ");
                        if (!DateTime.TryParse(
                            Console.ReadLine(), out DateTime admissionDate))
                        {
                            Console.WriteLine("Invalid admission date.");
                            break;
                        }

                        student.AdmissionDate = admissionDate;

                        bool updated = await service.UpdateAsync(student);

                        Console.WriteLine(updated
                            ? "Student updated successfully!"
                            : "Student could not be updated.");

                        break;
                    }
                case "6":
                    {
                        Console.Write("Enter Student ID to deactivate: ");

                        if (!int.TryParse(Console.ReadLine(), out int studentId) ||
                            studentId <= 0)
                        {
                            Console.WriteLine("Invalid student ID.");
                            break;
                        }

                        Student? student = await service.GetByIdAsync(studentId);

                        if (student == null)
                        {
                            Console.WriteLine("Active student not found.");
                            break;
                        }

                        Console.Write(
                            $"Deactivate {student.Name}? (y/n): ");

                        if (!(Console.ReadLine() ?? "").Equals(
                            "y", StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine("Deactivation cancelled.");
                            break;
                        }

                        bool deactivated =
                            await service.DeactivateAsync(studentId);

                        Console.WriteLine(deactivated
                            ? "Student deactivated successfully!"
                            : "Student could not be deactivated.");

                        break;
                    }

                case "7":
                    {
                        Console.Write("Enter Department ID: ");

                        if (!int.TryParse(Console.ReadLine(), out int departmentId) ||
                            departmentId <= 0)
                        {
                            Console.WriteLine("Invalid department ID.");
                            break;
                        }

                        List<Student> students =
                            await service.GetByDepartmentAsync(departmentId);

                        if (students.Count == 0)
                        {
                            Console.WriteLine(
                                "No active students found in this department.");
                            break;
                        }

                        foreach (Student student in students)
                        {
                            PrintStudent(student);
                        }

                        break;
                    }

                case "0":
                    running = false;
                    break;

                default:
                    Console.WriteLine("Invalid choice. Try again.");
                    break;
            }
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Validation error: {ex.Message}");
        }
        catch (SqlException ex)
        {
            if (ex.Number == 2601 || ex.Number == 2627)
            {
                Console.WriteLine(
                    "Database error: This email or value already exists.");
            }
            else
            {
                Console.WriteLine($"Database error: {ex.Message}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
        }
    }
}

static void PrintStudent(Student student)
{
    Console.WriteLine(
        $"ID: {student.StudentId} | " +
        $"Name: {student.Name} | " +
        $"Email: {student.Email} | " +
        $"Age: {student.Age} | " +
        $"Department ID: {student.DepartmentId}");
}

static async Task DepartmentMenu(DepartmentService service)
{
    bool running = true;

    while (running)
    {
        Console.WriteLine();
        Console.WriteLine("================================");
        Console.WriteLine(" DEPARTMENT MANAGEMENT");
        Console.WriteLine("================================");
        Console.WriteLine("1. Add Department");
        Console.WriteLine("2. List All Departments");
        Console.WriteLine("3. Search Department by ID");
        Console.WriteLine("4. Update Department");
        Console.WriteLine("5. Deactivate Department");
        Console.WriteLine("0. Back to Main Menu");
        Console.Write("Enter your choice: ");

        string? choice = Console.ReadLine();

        try
        {
            switch (choice)
            {
                case "1":
                    {
                        Console.Write("Enter department name: ");
                        string name = Console.ReadLine() ?? "";

                        Console.Write("Enter location (optional): ");
                        string? location = Console.ReadLine();

                        Department department = new()
                        {
                            DepartmentName = name,
                            Location = string.IsNullOrWhiteSpace(location)
                                ? null
                                : location
                        };

                        int id = await service.AddAsync(department);

                        Console.WriteLine(
                            $"Department added successfully! ID: {id}");

                        break;
                    }

                case "2":
                    {
                        List<Department> departments =
                            await service.GetAllAsync();

                        if (departments.Count == 0)
                        {
                            Console.WriteLine("No departments found.");
                            break;
                        }

                        foreach (Department department in departments)
                        {
                            PrintDepartment(department);
                        }

                        break;
                    }

                case "3":
                    {
                        Console.Write("Enter department ID: ");

                        if (!int.TryParse(Console.ReadLine(), out int id))
                        {
                            Console.WriteLine("Department ID must be a number.");
                            break;
                        }

                        Department? department = await service.GetByIdAsync(id);

                        if (department == null)
                        {
                            Console.WriteLine("Department not found.");
                        }
                        else
                        {
                            PrintDepartment(department);
                        }

                        break;
                    }

                case "4":
                    {
                        Console.Write("Enter department ID to update: ");

                        if (!int.TryParse(Console.ReadLine(), out int id) || id <= 0)
                        {
                            Console.WriteLine("Invalid department ID.");
                            break;
                        }

                        Department? department = await service.GetByIdAsync(id);

                        if (department == null || !department.IsActive)
                        {
                            Console.WriteLine("Active department not found.");
                            break;
                        }

                        Console.Write("Enter new department name: ");
                        department.DepartmentName = Console.ReadLine() ?? "";

                        Console.Write("Enter new location (optional): ");
                        string? location = Console.ReadLine();

                        department.Location = string.IsNullOrWhiteSpace(location)
                            ? null
                            : location;

                        bool updated = await service.UpdateAsync(department);

                        Console.WriteLine(updated
                            ? "Department updated successfully!"
                            : "Department could not be updated.");

                        break;
                    }

                case "5":
                    {
                        Console.Write("Enter department ID to deactivate: ");

                        if (!int.TryParse(Console.ReadLine(), out int id) || id <= 0)
                        {
                            Console.WriteLine("Invalid department ID.");
                            break;
                        }

                        Department? department = await service.GetByIdAsync(id);

                        if (department == null || !department.IsActive)
                        {
                            Console.WriteLine("Active department not found.");
                            break;
                        }

                        Console.Write(
                            $"Deactivate {department.DepartmentName}? (y/n): ");

                        if (!(Console.ReadLine() ?? "").Equals(
                            "y", StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine("Deactivation cancelled.");
                            break;
                        }

                        bool deactivated = await service.DeactivateAsync(id);

                        Console.WriteLine(deactivated
                            ? "Department deactivated successfully!"
                            : "Department could not be deactivated.");

                        break;
                    }

                case "0":
                    running = false;
                    break;

                default:
                    Console.WriteLine("Invalid choice. Try again.");
                    break;
            }
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Validation error: {ex.Message}");
        }
        catch (SqlException ex)
        {
            if (ex.Number == 2601 || ex.Number == 2627)
            {
                Console.WriteLine(
                    "Database error: That department name already exists.");
            }
            else
            {
                Console.WriteLine($"Database error: {ex.Message}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
        }
    }
}
static void PrintDepartment(Department department)
{
    Console.WriteLine(
        $"ID: {department.DepartmentId} | " +
        $"Name: {department.DepartmentName} | " +
        $"Location: {department.Location ?? "Not specified"} | " +
        $"Active: {department.IsActive}");
}

static async Task CourseMenu(CourseService service)
{
    bool running = true;

    while (running)
    {
        Console.WriteLine();
        Console.WriteLine("================================");
        Console.WriteLine(" COURSE MANAGEMENT");
        Console.WriteLine("================================");
        Console.WriteLine("1. Add Course");
        Console.WriteLine("2. List All Courses");
        Console.WriteLine("3. Search Course by ID");
        Console.WriteLine("4. Filter Courses by Department");
        Console.WriteLine("5. Update Course");
        Console.WriteLine("6. Deactivate Course");
        Console.WriteLine("0. Back to Main Menu");
        Console.Write("Enter your choice: ");

        string? choice = Console.ReadLine();

        try
        {
            switch (choice)
            {
                case "1":
                    {
                        Console.Write("Enter course name: ");
                        string courseName = Console.ReadLine() ?? "";

                        Console.Write("Enter duration in months: ");
                        if (!int.TryParse(Console.ReadLine(), out int duration))
                        {
                            Console.WriteLine("Duration must be a number.");
                            break;
                        }

                        Console.Write("Enter course fee: ");
                        if (!decimal.TryParse(Console.ReadLine(), out decimal fee))
                        {
                            Console.WriteLine("Fee must be a valid number.");
                            break;
                        }

                        Console.Write("Enter department ID: ");
                        if (!int.TryParse(Console.ReadLine(), out int departmentId))
                        {
                            Console.WriteLine("Department ID must be a number.");
                            break;
                        }

                        Course course = new()
                        {
                            CourseName = courseName,
                            DurationMonths = duration,
                            Fee = fee,
                            DepartmentId = departmentId
                        };

                        await service.AddCourseAsync(course);
                        Console.WriteLine("Course added successfully!");
                        break;
                    }

                case "2":
                    {
                        List<Course> courses = await service.GetAllCoursesAsync();

                        if (courses.Count == 0)
                        {
                            Console.WriteLine("No courses found.");
                            break;
                        }

                        foreach (Course course in courses)
                        {
                            PrintCourse(course);
                        }

                        break;
                    }

                case "3":
                    {
                        Console.Write("Enter course ID: ");

                        if (!int.TryParse(Console.ReadLine(), out int id) || id <= 0)
                        {
                            Console.WriteLine("Enter a valid course ID.");
                            break;
                        }

                        Course? course = await service.GetCourseByIdAsync(id);

                        if (course == null)
                        {
                            Console.WriteLine("Course not found.");
                        }
                        else
                        {
                            PrintCourse(course);
                        }

                        break;
                    }

                case "4":
                    {
                        Console.Write("Enter department ID: ");

                        if (!int.TryParse(Console.ReadLine(), out int departmentId) ||
                            departmentId <= 0)
                        {
                            Console.WriteLine("Enter a valid department ID.");
                            break;
                        }

                        List<Course> courses =
                            await service.GetCoursesByDepartmentAsync(departmentId);

                        if (courses.Count == 0)
                        {
                            Console.WriteLine("No courses found for this department.");
                            break;
                        }

                        foreach (Course course in courses)
                        {
                            PrintCourse(course);
                        }

                        break;
                    }
                case "5":
                    {
                        Console.Write("Enter course ID to update: ");

                        if (!int.TryParse(Console.ReadLine(), out int id) || id <= 0)
                        {
                            Console.WriteLine("Enter a valid course ID.");
                            break;
                        }

                        Course? course = await service.GetCourseByIdAsync(id);

                        if (course == null || !course.IsActive)
                        {
                            Console.WriteLine("Active course not found.");
                            break;
                        }

                        Console.Write("Enter new course name: ");
                        course.CourseName = Console.ReadLine() ?? "";

                        Console.Write("Enter new duration in months: ");

                        if (!int.TryParse(Console.ReadLine(), out int duration))
                        {
                            Console.WriteLine("Duration must be a number.");
                            break;
                        }

                        course.DurationMonths = duration;

                        Console.Write("Enter new fee: ");

                        if (!decimal.TryParse(Console.ReadLine(), out decimal fee))
                        {
                            Console.WriteLine("Fee must be a valid number.");
                            break;
                        }

                        course.Fee = fee;

                        Console.Write("Enter new department ID: ");

                        if (!int.TryParse(Console.ReadLine(), out int departmentId) ||
                            departmentId <= 0)
                        {
                            Console.WriteLine("Enter a valid department ID.");
                            break;
                        }

                        course.DepartmentId = departmentId;

                        await service.UpdateCourseAsync(course);
                        Console.WriteLine("Course updated successfully!");
                        break;
                    }

                case "6":
                    {
                        Console.Write("Enter course ID to deactivate: ");

                        if (!int.TryParse(Console.ReadLine(), out int id) || id <= 0)
                        {
                            Console.WriteLine("Enter a valid course ID.");
                            break;
                        }

                        await service.DeactivateCourseAsync(id);
                        Console.WriteLine("Course deactivated successfully!");
                        break;
                    }

                case "0":
                    running = false;
                    break;

                default:
                    Console.WriteLine("Invalid choice. Try again.");
                    break;
            }
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Validation error: {ex.Message}");
        }
        catch (SqlException ex)
        {
            if (ex.Number == 2601 || ex.Number == 2627)
            {
                Console.WriteLine("Database error: That course name already exists.");
            }
            else if (ex.Number == 547)
            {
                Console.WriteLine(
                    "Database error: Check that the department exists and is valid.");
            }
            else
            {
                Console.WriteLine($"Database error: {ex.Message}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}

static async Task EnrollmentMenu(EnrollmentService service)
{
    bool running = true;

    while (running)
    {
        Console.WriteLine();
        Console.WriteLine("================================");
        Console.WriteLine(" ENROLLMENT MANAGEMENT");
        Console.WriteLine("================================");
        Console.WriteLine("1. Enroll Student in Course");
        Console.WriteLine("2. List All Enrollments");
        Console.WriteLine("3. Search Enrollment by ID");
        Console.WriteLine("4. Update Enrollment Status");
        Console.WriteLine("0. Back to Main Menu");
        Console.Write("Enter your choice: ");

        string? choice = Console.ReadLine();

        try
        {
            switch (choice)
            {
                case "1":
                    {
                        Console.Write("Enter Student ID: ");

                        if (!int.TryParse(Console.ReadLine(), out int studentId)
                            || studentId <= 0)
                        {
                            Console.WriteLine("Invalid Student ID.");
                            break;
                        }

                        Console.Write("Enter Course ID: ");

                        if (!int.TryParse(Console.ReadLine(), out int courseId)
                            || courseId <= 0)
                        {
                            Console.WriteLine("Invalid Course ID.");
                            break;
                        }

                        Enrollment enrollment = new()
                        {
                            StudentId = studentId,
                            CourseId = courseId,
                            Status = "Enrolled"
                        };

                        int id = await service.EnrollAsync(enrollment);

                        Console.WriteLine(
                            $"Enrollment successful! Enrollment ID: {id}");
                        break;
                    }

                case "2":
                    {
                        List<Enrollment> enrollments =
                            await service.GetAllAsync();

                        if (enrollments.Count == 0)
                        {
                            Console.WriteLine("No enrollments found.");
                            break;
                        }

                        foreach (Enrollment e in enrollments)
                        {
                            PrintEnrollment(e);
                        }

                        break;
                    }

                case "3":
                    {
                        Console.Write("Enter Enrollment ID: ");

                        if (!int.TryParse(Console.ReadLine(), out int id)
                            || id <= 0)
                        {
                            Console.WriteLine("Invalid Enrollment ID.");
                            break;
                        }

                        Enrollment? enrollment =
                            await service.GetByIdAsync(id);

                        if (enrollment == null)
                        {
                            Console.WriteLine("Enrollment not found.");
                        }
                        else
                        {
                            PrintEnrollment(enrollment);
                        }

                        break;
                    }

                case "4":
                    {
                        Console.Write("Enter Enrollment ID: ");

                        if (!int.TryParse(Console.ReadLine(), out int id)
                            || id <= 0)
                        {
                            Console.WriteLine("Invalid Enrollment ID.");
                            break;
                        }

                        Console.WriteLine("Valid statuses: Enrolled, Completed, Cancelled");
                        Console.Write("Enter new status: ");
                        string status = Console.ReadLine() ?? "";

                        bool updated =
                            await service.UpdateStatusAsync(id, status);

                        Console.WriteLine(updated
                            ? "Enrollment status updated successfully!"
                            : "Enrollment not found.");

                        break;
                    }

                case "0":
                    running = false;
                    break;

                default:
                    Console.WriteLine("Invalid choice. Try again.");
                    break;
            }
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Validation error: {ex.Message}");
        }
        catch (SqlException ex)
        {
            if (ex.Number == 2601 || ex.Number == 2627)
            {
                Console.WriteLine(
                    "This student is already enrolled in that course.");
            }
            else if (ex.Number == 547)
            {
                Console.WriteLine(
                    "Student ID or Course ID does not exist.");
            }
            else
            {
                Console.WriteLine($"Database error: {ex.Message}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}

static void PrintEnrollment(Enrollment enrollment)
{
    Console.WriteLine(
        $"Enrollment ID: {enrollment.EnrollmentId} | " +
        $"Student ID: {enrollment.StudentId} | " +
        $"Course ID: {enrollment.CourseId} | " +
        $"Date: {enrollment.EnrolledAt} | " +
        $"Status: {enrollment.Status}");
}
static async Task ReportMenu(ReportService service)
{
    while (true)
    {
        Console.WriteLine("\n===== REPORTS =====");
        Console.WriteLine("1. Student Count by Department");
        Console.WriteLine("2. Enrollment Count by Course");
        Console.WriteLine("3. Students Without Enrollments");
        Console.WriteLine("4. Courses Without Enrollments");
        Console.WriteLine("0. Back to Main Menu");
        Console.Write("Choose an option: ");

        string? choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                {
                    var reports =
                        await service.GetStudentCountByDepartmentAsync();

                    Console.WriteLine("\n--- Student Count by Department ---");

                    if (reports.Count == 0)
                    {
                        Console.WriteLine("No departments found.");
                        break;
                    }

                    foreach (var report in reports)
                    {
                        Console.WriteLine(
                            $"{report.DepartmentName}: {report.StudentCount} students");
                    }

                    break;
                }

            case "2":
                {
                    var reports =
                        await service.GetEnrollmentCountByCourseAsync();

                    Console.WriteLine("\n--- Enrollment Count by Course ---");

                    if (reports.Count == 0)
                    {
                        Console.WriteLine("No courses found.");
                        break;
                    }

                    foreach (var report in reports)
                    {
                        Console.WriteLine(
                            $"{report.CourseName}: {report.EnrollmentCount} enrollments");
                    }

                    break;
                }

            case "3":
                {
                    var reports =
                        await service.GetStudentsWithoutEnrollmentsAsync();

                    Console.WriteLine("\n--- Students Without Enrollments ---");

                    if (reports.Count == 0)
                    {
                        Console.WriteLine("All students have enrollments, or no students exist.");
                        break;
                    }

                    foreach (var report in reports)
                    {
                        Console.WriteLine(
                            $"ID: {report.StudentId}, Name: {report.Name}");
                    }

                    break;
                }

            case "4":
                {
                    var reports =
                        await service.GetCoursesWithoutEnrollmentsAsync();

                    Console.WriteLine("\n--- Courses Without Enrollments ---");

                    if (reports.Count == 0)
                    {
                        Console.WriteLine("All courses have enrollments, or no courses exist.");
                        break;
                    }

                    foreach (var report in reports)
                    {
                        Console.WriteLine(
                            $"ID: {report.CourseId}, Course: {report.CourseName}");
                    }

                    break;
                }

            case "0":
                return;

            default:
                Console.WriteLine("Invalid option. Try again.");
                break;
        }
    }
}
static void PrintCourse(Course course)
{
    Console.WriteLine(
        $"ID: {course.CourseId} | " +
        $"Name: {course.CourseName} | " +
        $"Duration: {course.DurationMonths} months | " +
        $"Fee: {course.Fee:C} | " +
        $"Department ID: {course.DepartmentId} | " +
        $"Active: {course.IsActive}");
}
static async Task ViewAuditLogs(AuditLogService service)
{
    Console.WriteLine("\n===== RECENT AUDIT LOGS =====");

    var logs = await service.GetRecentAsync();

    if (logs.Count == 0)
    {
        Console.WriteLine("No audit logs found.");
        return;
    }

    foreach (var log in logs)
    {
        Console.WriteLine(
            $"ID: {log.AuditId} | " +
            $"Action: {log.Action} | " +
            $"Entity: {log.EntityName} | " +
            $"Entity ID: {log.EntityId?.ToString() ?? "N/A"}");

        Console.WriteLine($"Details: {log.Details}");
        Console.WriteLine($"Date: {log.CreatedAt}");
        Console.WriteLine("----------------------------------------");
    }
}