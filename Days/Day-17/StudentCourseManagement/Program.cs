
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


        case "0":
            running = false;
            Console.WriteLine("Application closed.");
            break;

        default:
            Console.WriteLine("Invalid choice. Try again.");
            break;
    }
}
