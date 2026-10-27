
using StudentCourseManagement.Models;
using StudentCourseManagement.Repositories;
using StudentCourseManagement.Services;

StudentRepository repository = new();
StudentService service = new(repository);

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
    Console.WriteLine("0. Exit");
    Console.Write("Enter your choice: ");

    string? choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            try
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
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Validation error: {ex.Message}");
            }
            catch (Microsoft.Data.SqlClient.SqlException ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }

            break;

        case "2":
            try
            {
                List<Student> students = await service.GetAllAsync();

                if (students.Count == 0)
                {
                    Console.WriteLine("No active students found.");
                    break;
                }

                Console.WriteLine();
                Console.WriteLine("===== ACTIVE STUDENTS =====");

                foreach (Student student in students)
                {
                    Console.WriteLine(
                        $"ID: {student.StudentId} | " +
                        $"Name: {student.Name} | " +
                        $"Email: {student.Email} | " +
                        $"Age: {student.Age} | " +
                        $"Department ID: {student.DepartmentId}");
                }
            }
            catch (Microsoft.Data.SqlClient.SqlException ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
            }
            break;

        case "3":
            try
            {
                Console.Write("Enter student ID: ");

                if (!int.TryParse(Console.ReadLine(), out int studentId))
                {
                    Console.WriteLine("Student ID must be a number.");
                    break;
                }

                Student? student = await service.GetByIdAsync(studentId);

                if (student is null)
                {
                    Console.WriteLine("No active student found with that ID.");
                    break;
                }

                Console.WriteLine();
                Console.WriteLine("===== STUDENT DETAILS =====");
                Console.WriteLine($"ID: {student.StudentId}");
                Console.WriteLine($"Name: {student.Name}");
                Console.WriteLine($"Email: {student.Email}");
                Console.WriteLine($"Age: {student.Age}");
                Console.WriteLine($"Department ID: {student.DepartmentId}");
                Console.WriteLine($"Admission Date: {student.AdmissionDate}");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine($"Validation error: {ex.Message}");
            }
            catch (Microsoft.Data.SqlClient.SqlException ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
            }
            break;



        case "4":
            try
            {
                Console.Write("Enter student name to search: ");
                string name = Console.ReadLine() ?? "";

                List<Student> students =
                    await service.SearchByNameAsync(name);

                if (students.Count == 0)
                {
                    Console.WriteLine("No active students found matching that name.");
                    break;
                }

                Console.WriteLine();
                Console.WriteLine("===== SEARCH RESULTS =====");

                foreach (Student student in students)
                {
                    Console.WriteLine(
                        $"ID: {student.StudentId} | " +
                        $"Name: {student.Name} | " +
                        $"Email: {student.Email} | " +
                        $"Age: {student.Age} | " +
                        $"Department ID: {student.DepartmentId}");
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Validation error: {ex.Message}");
            }
            catch (Microsoft.Data.SqlClient.SqlException ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
            }
            break;

        case "5":
            try
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

                Console.Write("Enter new admission date (yyyy-MM-dd): ");

                if (!DateTime.TryParse(
                        Console.ReadLine(),
                        out DateTime admissionDate))
                {
                    Console.WriteLine("Invalid admission date.");
                    break;
                }

                student.AdmissionDate = admissionDate;

                bool updated = await service.UpdateAsync(student);

                Console.WriteLine(updated
                    ? "Student updated successfully!"
                    : "Student could not be updated.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Validation error: {ex.Message}");
            }
            catch (Microsoft.Data.SqlClient.SqlException ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
            }

            break;

        case "6":
            try
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
                    $"Are you sure you want to deactivate {student.Name}? (y/n): ");

                string confirmation = Console.ReadLine() ?? "";

                if (!confirmation.Equals(
                        "y",
                        StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Deactivation cancelled.");
                    break;
                }

                bool deactivated = await service.DeactivateAsync(studentId);

                Console.WriteLine(deactivated
                    ? "Student deactivated successfully!"
                    : "Student could not be deactivated.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Validation error: {ex.Message}");
            }
            catch (Microsoft.Data.SqlClient.SqlException ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
            }

            break;

        case "7":
            try
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

                Console.WriteLine("\nActive Students in Department:");

                foreach (Student student in students)
                {
                    Console.WriteLine(
                        $"ID: {student.StudentId} | " +
                        $"Name: {student.Name} | " +
                        $"Email: {student.Email} | " +
                        $"Age: {student.Age}");
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Validation error: {ex.Message}");
            }
            catch (Microsoft.Data.SqlClient.SqlException ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
            }

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
