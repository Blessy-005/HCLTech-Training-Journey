
using Microsoft.Data.SqlClient;
using StudentCourseManagement.Models;
using StudentCourseManagement.Repositories;
using StudentCourseManagement.Services;

StudentRepository studentRepository = new();
StudentService studentService = new(studentRepository);

DepartmentRepository departmentRepository = new();
DepartmentService departmentService = new(departmentRepository);

bool running = true;

while (running)
{
    Console.WriteLine();
    Console.WriteLine("================================");
    Console.WriteLine(" MAIN MENU");
    Console.WriteLine("================================");
    Console.WriteLine("1. Student Management");
    Console.WriteLine("2. Department Management");
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
