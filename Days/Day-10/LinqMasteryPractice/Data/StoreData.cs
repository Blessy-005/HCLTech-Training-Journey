using LinqMastery.Models;

namespace LinqMastery.Data;

public static class StoreData
{
    public static List<Product> Products = new()
    {
        new() { ProductId = 1, ProductName = "Laptop", Category = "Electronics", Price = 60000m, Stock = 10, SupplierId = 1, Tags = new() { "Computer", "Premium" } },
        new() { ProductId = 2, ProductName = "Mouse", Category = "Electronics", Price = 800m, Stock = 50, SupplierId = 1, Tags = new() { "Accessory", "Wireless" } },
        new() { ProductId = 3, ProductName = "Keyboard", Category = "Electronics", Price = 1500m, Stock = 30, SupplierId = 2, Tags = new() { "Accessory", "Gaming" } },
        new() { ProductId = 4, ProductName = "Office Chair", Category = "Furniture", Price = 7500m, Stock = 8, SupplierId = 2, Tags = new() { "Office", "Comfort" } },
        new() { ProductId = 5, ProductName = "Office Desk", Category = "Furniture", Price = 12000m, Stock = 5, SupplierId = 3, Tags = new() { "Office", "Wooden" } },
        new() { ProductId = 6, ProductName = "Monitor", Category = "Electronics", Price = 18000m, Stock = 0, SupplierId = 1, Tags = new() { "Computer", "Display" } },
        new() { ProductId = 7, ProductName = "Notebook", Category = "Stationery", Price = 100m, Stock = 100, SupplierId = 4, Tags = new() { "Paper", "Office" } },
        new() { ProductId = 8, ProductName = "Pen", Category = "Stationery", Price = 25m, Stock = 200, SupplierId = 4, Tags = new() { "Writing", "Office" } }
    };

    public static List<Customer> Customers = new()
    {
        new() { CustomerId = 1, CustomerName = "Arun", City = "Chennai", Email = "arun@example.com" },
        new() { CustomerId = 2, CustomerName = "Priya", City = "Coimbatore", Email = "priya@example.com" },
        new() { CustomerId = 3, CustomerName = "Kumar", City = "Chennai", Email = "kumar@example.com" },
        new() { CustomerId = 4, CustomerName = "Divya", City = "Madurai", Email = "divya@example.com" },
        new() { CustomerId = 5, CustomerName = "Ravi", City = "Salem", Email = "ravi@example.com" }
    };

    public static List<Order> Orders = new()
    {
        new()
        {
            OrderId = 101, CustomerId = 1, OrderDate = new DateTime(2026, 1, 10),
            Status = "Delivered", Discount = 500m,
            Items = new()
            {
                new() { ProductId = 1, Quantity = 1, UnitPrice = 60000m },
                new() { ProductId = 2, Quantity = 2, UnitPrice = 800m }
            }
        },
        new()
        {
            OrderId = 102, CustomerId = 2, OrderDate = new DateTime(2026, 1, 12),
            Status = "Pending", Discount = 0m,
            Items = new() { new() { ProductId = 4, Quantity = 2, UnitPrice = 7500m } }
        },
        new()
        {
            OrderId = 103, CustomerId = 1, OrderDate = new DateTime(2026, 2, 2),
            Status = "Delivered", Discount = 1000m,
            Items = new()
            {
                new() { ProductId = 3, Quantity = 2, UnitPrice = 1500m },
                new() { ProductId = 7, Quantity = 10, UnitPrice = 100m }
            }
        },
        new()
        {
            OrderId = 104, CustomerId = 3, OrderDate = new DateTime(2026, 2, 15),
            Status = "Cancelled", Discount = 0m,
            Items = new() { new() { ProductId = 5, Quantity = 1, UnitPrice = 12000m } }
        },
        new()
        {
            OrderId = 105, CustomerId = 4, OrderDate = new DateTime(2026, 3, 1),
            Status = "Pending", Discount = 200m,
            Items = new() { new() { ProductId = 8, Quantity = 20, UnitPrice = 25m } }
        },
        new()
        {
            OrderId = 106, CustomerId = 99, OrderDate = new DateTime(2026, 3, 5),
            Status = "Pending", Discount = 0m,
            Items = new() { new() { ProductId = 99, Quantity = 1, UnitPrice = 500m } }
        }
    };

    public static List<Department> Departments = new()
    {
        new() { DepartmentId = 10, DepartmentName = "IT" },
        new() { DepartmentId = 20, DepartmentName = "HR" },
        new() { DepartmentId = 30, DepartmentName = "Sales" },
        new() { DepartmentId = 40, DepartmentName = "Finance" }
    };

    public static List<Employee> Employees = new()
    {
        new() { EmployeeId = 1, EmployeeName = "Arun", DepartmentId = 10, Salary = 60000m },
        new() { EmployeeId = 2, EmployeeName = "Priya", DepartmentId = 20, Salary = 45000m },
        new() { EmployeeId = 3, EmployeeName = "Kumar", DepartmentId = 10, Salary = 75000m },
        new() { EmployeeId = 4, EmployeeName = "Divya", DepartmentId = 30, Salary = 50000m },
        // No matching department: intentionally included for left-join practice.
        new() { EmployeeId = 5, EmployeeName = "Ravi", DepartmentId = 99, Salary = 40000m }
    };

    public static List<int> ListA = new() { 10, 20, 20, 30, 40, 50 };
    public static List<int> ListB = new() { 30, 40, 40, 60, 70 };

    public static List<List<string>> TrainingBatches = new()
    {
        new() { "C#", "LINQ", "SQL" },
        new() { "Java", "Spring Boot" },
        new() { "Python", "Django", "SQL" }
    };
}
