using LinqMastery.Data;
using System.Linq;

namespace LinqMastery;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("LINQ Mastery Practice Project");

        // Run() prints a heading, then runs the exercise.
        // If one exercise throws (e.g. Single() finds 0 or 2 items), the others still run.
        Run(Exercise01_Filtering);
        Run(Exercise02_Projection);
        Run(Exercise03_SelectMany);
        Run(Exercise04_Sorting);
        Run(Exercise05_ElementOperators);
        Run(Exercise06_Aggregation);
        Run(Exercise07_AnyAll);
        Run(Exercise08_Grouping);
        Run(Exercise09_SetOperations);
        Run(Exercise10_InnerJoin);
        Run(Exercise11_LeftJoin);
        Run(Exercise12_GroupJoin);
        Run(Exercise13_Partitioning);
        Run(Exercise14_QuantifiersAndMembership);
        Run(Exercise15_StringAndNumericQueries);
        Run(Exercise16_NestedQueries);
        Run(Exercise17_CombinedQueries);
        Run(Exercise18_DictionaryAndLookup);
        Run(Exercise19_JoinOrdersAndCustomers);
        Run(Exercise20_Reports);

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }

    // ---------------------------------------------------------------
    // Helpers: keep the printing code short so the LINQ stays the focus
    // ---------------------------------------------------------------

    static void Run(Action exercise)
    {
        Console.WriteLine();
        Console.WriteLine(new string('=', 60));
        Console.WriteLine(exercise.Method.Name);
        Console.WriteLine(new string('=', 60));

        try
        {
            exercise();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  !! {ex.GetType().Name}: {ex.Message}");
        }
    }
    // Prints a title and every item of any sequence (the query runs here).
    static void Show<T>(string title, IEnumerable<T> items)
    {
        Console.WriteLine($"\n{title}");

        bool any = false;
        foreach (T item in items)
        {
            Console.WriteLine($"  {item}");
            any = true;
        }

        if (!any)
        {
            Console.WriteLine("  (no results)");
        }
    }

    // Prints a title and a single value.
    static void ShowValue<T>(string title, T value)
    {
        Console.WriteLine($"\n{title}\n  {value}");
    }

    // RULE: For every task, implement both styles where supported.
    // Use these labels to keep your work organised:
    // METHOD SYNTAX PRACTICE
    // QUERY SYNTAX PRACTICE

    static void Exercise01_Filtering()
    {
        var products = StoreData.Products;

        // Task 1: Find products priced above 5,000.
        // METHOD SYNTAX PRACTICE
        var filteredProductsMethod1 = products.Where(p => p.Price > 5000);
        // QUERY SYNTAX PRACTICE
        var filteredProductsQuery1 = from p in products where p.Price > 5000 select p;

        // Task 2: Find Electronics products that are in stock.
        // METHOD SYNTAX PRACTICE
        var filteredProductsMethod2 = products.Where(p => p.Category == "Electronics" && p.Stock > 0);
        // QUERY SYNTAX PRACTICE
        var filteredProductsQuery2 = from p in products where p.Category == "Electronics" && p.Stock > 0 select p;

        // Task 3: Find products priced between 1,000 and 20,000.
        // METHOD SYNTAX PRACTICE
        var filteredProductsMethod3 = products.Where(p => p.Price >= 1000 && p.Price <= 20000);
        // QUERY SYNTAX PRACTICE
        var filteredProductsQuery3 = from p in products where p.Price >= 1000 && p.Price <= 20000 select p;

        // Task 4: Find products whose names contain "Office".
        // METHOD SYNTAX PRACTICE
        var filteredProductsMethod4 = products.Where(p => p.ProductName.Contains("Office"));
        // QUERY SYNTAX PRACTICE
        var filteredProductsQuery4 = from p in products where p.ProductName.Contains("Office") select p;

        Console.WriteLine("\nTask 1: Method Syntax - Products above 5000");
        foreach (var p in filteredProductsMethod1)
            Console.WriteLine($"{p.ProductName} - {p.Price}");

        Console.WriteLine("\nTask 1: Query Syntax - Products above 5000");
        foreach (var p in filteredProductsQuery1)
            Console.WriteLine($"{p.ProductName} - {p.Price}");

        Console.WriteLine(new string('-', 30));

        Console.WriteLine("\nTask 2: Method Syntax - Electronics in stock");
        foreach (var p in filteredProductsMethod2)
            Console.WriteLine($"{p.ProductName} - {p.Stock}");

        Console.WriteLine("\nTask 2: Query Syntax - Electronics in stock");
        foreach (var p in filteredProductsQuery2)
            Console.WriteLine($"{p.ProductName} - {p.Stock}");

        Console.WriteLine(new string('-', 30));

        Console.WriteLine("\nTask 3: Method Syntax - Products between 1000 and 20000");
        foreach (var p in filteredProductsMethod3)
            Console.WriteLine($"{p.ProductName} - {p.Price}");

        Console.WriteLine("\nTask 3: Query Syntax - Products between 1000 and 20000");
        foreach (var p in filteredProductsQuery3)
            Console.WriteLine($"{p.ProductName} - {p.Price}");

        Console.WriteLine(new string('-', 30));

        Console.WriteLine("\nTask 4: Method Syntax - Products containing Office");
        foreach (var p in filteredProductsMethod4)
            Console.WriteLine($"{p.ProductName} - {p.Price}");

        Console.WriteLine("\nTask 4: Query Syntax - Products containing Office");
        foreach (var p in filteredProductsQuery4)
            Console.WriteLine($"{p.ProductName} - {p.Price}");
    }

    static void Exercise02_Projection()
    {
        var products = StoreData.Products;

        // Task 1: Select only product names.
        // METHOD SYNTAX PRACTICE
        var productNamesMethod = products.Select(p => p.ProductName);
        Show("Task 1: Method Syntax - Product Names", productNamesMethod);

        // QUERY SYNTAX PRACTICE
        var productNamesQuery = from p in products select p.ProductName;
        Show("Task 1: Query Syntax - Product Names", productNamesQuery);

        // Task 2: Return ProductName and Price using an anonymous type.
        // METHOD SYNTAX PRACTICE
        var productDetailsMethod = products.Select(p => new { p.ProductName, p.Price });
        Console.WriteLine("\nTask 2: Method Syntax - Product Details");
        foreach (var p in productDetailsMethod)
        {
            Console.WriteLine($"  {p.ProductName} - {p.Price}");
        }

        // QUERY SYNTAX PRACTICE
        var productDetailsQuery = from p in products select new { p.ProductName, p.Price };
        Console.WriteLine("\nTask 2: Query Syntax - Product Details");
        foreach (var p in productDetailsQuery)
        {
            Console.WriteLine($"  {p.ProductName} - {p.Price}");
        }

        // Task 3: Return ProductName, Price, and PriceWithTax (18%).
        // METHOD SYNTAX PRACTICE
        var productsWithTaxMethod = products.Select(p => new { p.ProductName, p.Price, PriceWithTax = p.Price * 1.18m });
        Console.WriteLine("\nTask 3: Method Syntax - Price with 18% tax");
        foreach (var p in productsWithTaxMethod)
        {
            Console.WriteLine($"  {p.ProductName} - {p.Price} - {p.PriceWithTax:F2}");
        }

        // QUERY SYNTAX PRACTICE
        var productsWithTaxQuery = from p in products select new { p.ProductName, p.Price, PriceWithTax = p.Price * 1.18m };
        Console.WriteLine("\nTask 3: Query Syntax - Price with 18% tax");
        foreach (var p in productsWithTaxQuery)
        {
            Console.WriteLine($"  {p.ProductName} - {p.Price} - {p.PriceWithTax:F2}");
        }
    }

    static void Exercise03_SelectMany()
    {
        // Task 1: Flatten all training batches into one sequence.
        var batches = StoreData.TrainingBatches;

        // METHOD SYNTAX PRACTICE
        var allBatchesMethod = batches.SelectMany(b => b);
        Show("Task 1: Method Syntax - All learners from all batches", allBatchesMethod);

        // QUERY SYNTAX PRACTICE
        var allBatchesQuery =
            from batch in batches
            from learner in batch
            select learner;
        Show("Task 1: Query Syntax - All learners from all batches", allBatchesQuery);

        // Task 2: Get every product tag as one flat sequence.
        // METHOD SYNTAX PRACTICE
        var allTagsMethod =
            StoreData.Products.SelectMany(p => p.Tags);
        Show("Task 2: Method Syntax - All tags", allTagsMethod);

        // QUERY SYNTAX PRACTICE
        var allTagsQuery =
            from p in StoreData.Products
            from tag in p.Tags
            select tag;
        Show("Task 2: Query Syntax - All tags", allTagsQuery);
    }

    static void Exercise04_Sorting()
    {
        var products = StoreData.Products;

        // Task 1: Sort by price ascending.
        // METHOD SYNTAX PRACTICE
        var sortedProductsMethod1 = products.OrderBy(p => p.Price);
        Show("Task 1: Method Syntax - Price ascending",
            sortedProductsMethod1.Select(p => $"{p.ProductName} - {p.Price}"));

        // QUERY SYNTAX PRACTICE
        var sortedProductsQuery1 =
            from p in products
            orderby p.Price
            select p;
        Show("Task 1: Query Syntax - Price ascending",
            sortedProductsQuery1.Select(p => $"{p.ProductName} - {p.Price}"));

        // Task 2: Sort by price descending.
        // METHOD SYNTAX PRACTICE
        var sortedProductsMethod2 = products.OrderByDescending(p => p.Price);
        Show("Task 2: Method Syntax - Price descending",
            sortedProductsMethod2.Select(p => $"{p.ProductName} - {p.Price}"));

        // QUERY SYNTAX PRACTICE
        var sortedProductsQuery2 =
            from p in products
            orderby p.Price descending
            select p;
        Show("Task 2: Query Syntax - Price descending",
            sortedProductsQuery2.Select(p => $"{p.ProductName} - {p.Price}"));

        // Task 3: Sort by Category ascending, then Price descending.
        // METHOD SYNTAX PRACTICE
        var sortedProductsMethod3 = products
            .OrderBy(p => p.Category)
            .ThenByDescending(p => p.Price);
        Show("Task 3: Method Syntax - Category asc, Price desc",
            sortedProductsMethod3.Select(p => $"{p.Category} | {p.ProductName} - {p.Price}"));

        // QUERY SYNTAX PRACTICE
        var sortedProductsQuery3 =
            from p in products
            orderby p.Category ascending, p.Price descending
            select p;
        Show("Task 3: Query Syntax - Category asc, Price desc",
            sortedProductsQuery3.Select(p => $"{p.Category} | {p.ProductName} - {p.Price}"));
    }

    static void Exercise05_ElementOperators()
    {
        var products = StoreData.Products;

        // Task 1: Find the first product priced above 10,000.
        // METHOD SYNTAX PRACTICE
        var firstExpensiveMethod = products.First(p => p.Price > 10000);
        ShowValue("Task 1: Method Syntax - First product above 10,000",
            $"{firstExpensiveMethod.ProductName} - {firstExpensiveMethod.Price}");

        // QUERY SYNTAX PRACTICE
        var expensiveProductsQuery =
            from p in products
            where p.Price > 10000
            select p;

        var firstExpensiveQuery = expensiveProductsQuery.First();
        ShowValue("Task 1: Query Syntax - First product above 10,000",
            $"{firstExpensiveQuery.ProductName} - {firstExpensiveQuery.Price}");

        // Task 2: Search for ProductId 999 safely.
        // METHOD SYNTAX PRACTICE
        var product999Method = products.FirstOrDefault(p => p.ProductId == 999);
        ShowValue("Task 2: Method Syntax - ProductId 999",
            product999Method == null ? "Not found (FirstOrDefault returned null)" : product999Method.ProductName);

        // QUERY SYNTAX PRACTICE
        var product999QueryResult =
            (from p in products
             where p.ProductId == 999
             select p).FirstOrDefault();
        ShowValue("Task 2: Query Syntax - ProductId 999",
            product999QueryResult == null ? "Not found (FirstOrDefault returned null)" : product999QueryResult.ProductName);

        // Task 3: Find ProductId 3 using Single().
        // METHOD SYNTAX PRACTICE
        var product3Method = products.Single(p => p.ProductId == 3);
        ShowValue("Task 3: Method Syntax - ProductId 3", product3Method.ProductName);

        // QUERY SYNTAX PRACTICE
        var product3Query =
            (from p in products
             where p.ProductId == 3
             select p).Single();
        ShowValue("Task 3: Query Syntax - ProductId 3", product3Query.ProductName);
    }

    static void Exercise06_Aggregation()
    {
        var products = StoreData.Products;

        // Task 1: Count all products.
        // METHOD SYNTAX PRACTICE
        var productCountMethod = products.Count();

        // QUERY SYNTAX PRACTICE
        var productCountQuery =
            (from p in products
             select p).Count();

        Console.WriteLine($"\nTask 1: Product count - Method: {productCountMethod} | Query: {productCountQuery}");

        // Task 2: Calculate total stock.
        // METHOD SYNTAX PRACTICE
        var totalStockMethod = products.Sum(p => p.Stock);

        // QUERY SYNTAX PRACTICE
        var totalStockQuery =
            (from p in products
             select p.Stock).Sum();

        Console.WriteLine($"Task 2: Total stock - Method: {totalStockMethod} | Query: {totalStockQuery}");

        // Task 3: Find average, minimum and maximum price.
        // METHOD SYNTAX PRACTICE
        var averagePriceMethod = products.Average(p => p.Price);
        var minimumPriceMethod = products.Min(p => p.Price);
        var maximumPriceMethod = products.Max(p => p.Price);

        // QUERY SYNTAX PRACTICE
        var averagePriceQuery =
            (from p in products select p.Price).Average();

        var minimumPriceQuery =
            (from p in products select p.Price).Min();

        var maximumPriceQuery =
            (from p in products select p.Price).Max();

        Console.WriteLine($"Task 3: Method - Average: {averagePriceMethod:F2} | Min: {minimumPriceMethod} | Max: {maximumPriceMethod}");
        Console.WriteLine($"Task 3: Query  - Average: {averagePriceQuery:F2} | Min: {minimumPriceQuery} | Max: {maximumPriceQuery}");

        // Task 4: Calculate total inventory value (Price * Stock).
        // METHOD SYNTAX PRACTICE
        var inventoryValueMethod =
            products.Sum(p => p.Price * p.Stock);

        // QUERY SYNTAX PRACTICE
        var inventoryValueQuery =
            (from p in products
             select p.Price * p.Stock).Sum();

        Console.WriteLine($"Task 4: Inventory value - Method: {inventoryValueMethod:F2} | Query: {inventoryValueQuery:F2}");
    }

    static void Exercise07_AnyAll()
    {
        var products = StoreData.Products;

        // Task 1: Check whether any product costs more than 50,000.
        // METHOD SYNTAX PRACTICE
        var anyExpensiveMethod = products.Any(p => p.Price > 50000);

        // QUERY SYNTAX PRACTICE
        var anyExpensiveQuery =
            (from p in products
             where p.Price > 50000
             select p).Any();

        Console.WriteLine($"\nTask 1: Any product above 50,000? Method: {anyExpensiveMethod} | Query: {anyExpensiveQuery}");

        // Task 2: Check whether all products have non-negative stock.
        // METHOD SYNTAX PRACTICE
        var allStockValidMethod = products.All(p => p.Stock >= 0);

        // QUERY SYNTAX PRACTICE
        var allStockValidQuery =
            (from p in products
             select p.Stock >= 0).All(valid => valid);

        Console.WriteLine($"Task 2: All stock non-negative? Method: {allStockValidMethod} | Query: {allStockValidQuery}");
    }

    static void Exercise08_Grouping()
    {
        var products = StoreData.Products;

        // Task 1: Group products by Category.
        // METHOD SYNTAX PRACTICE
        var groupedProductsMethod = products.GroupBy(p => p.Category);

        Console.WriteLine("\nTask 1: Method Syntax - Products grouped by category");
        foreach (var group in groupedProductsMethod)
        {
            Console.WriteLine($"  {group.Key}");
            foreach (var p in group)
            {
                Console.WriteLine($"      {p.ProductName}");
            }
        }

        // QUERY SYNTAX PRACTICE
        var groupedProductsQuery =
            from p in products
            group p by p.Category;

        Console.WriteLine("\nTask 1: Query Syntax - Products grouped by category");
        foreach (var group in groupedProductsQuery)
        {
            Console.WriteLine($"  {group.Key}");
            foreach (var p in group)
            {
                Console.WriteLine($"      {p.ProductName}");
            }
        }

        // Task 2: For each category, calculate product count,
        // total stock, and average price.
        // METHOD SYNTAX PRACTICE
        var categorySummaryMethod = products
            .GroupBy(p => p.Category)
            .Select(g => new
            {
                Category = g.Key,
                ProductCount = g.Count(),
                TotalStock = g.Sum(p => p.Stock),
                AveragePrice = g.Average(p => p.Price)
            });

        Console.WriteLine("\nTask 2: Method Syntax - Category summary");
        foreach (var item in categorySummaryMethod)
        {
            Console.WriteLine($"  {item.Category} | Count: {item.ProductCount} | Stock: {item.TotalStock} | Avg price: {item.AveragePrice:F2}");
        }

        // QUERY SYNTAX PRACTICE
        var categorySummaryQuery =
            from p in products
            group p by p.Category into g
            select new
            {
                Category = g.Key,
                ProductCount = g.Count(),
                TotalStock = g.Sum(p => p.Stock),
                AveragePrice = g.Average(p => p.Price)
            };

        Console.WriteLine("\nTask 2: Query Syntax - Category summary");
        foreach (var item in categorySummaryQuery)
        {
            Console.WriteLine($"  {item.Category} | Count: {item.ProductCount} | Stock: {item.TotalStock} | Avg price: {item.AveragePrice:F2}");
        }
    } 
    static void Exercise09_SetOperations()
    {
        var a = StoreData.ListA;
        var b = StoreData.ListB;

        Console.WriteLine($"\nList A: {string.Join(", ", a)}");
        Console.WriteLine($"List B: {string.Join(", ", b)}");

        // Task 1: Remove duplicates.
        // METHOD SYNTAX PRACTICE
        var distinctMethod = a.Distinct();

        // QUERY SYNTAX PRACTICE
        var distinctQuery =
            (from item in a
             select item).Distinct();

        Console.WriteLine($"\nTask 1: Distinct A - Method: {string.Join(", ", distinctMethod)}");
        Console.WriteLine($"Task 1: Distinct A - Query:  {string.Join(", ", distinctQuery)}");

        // Task 2: Find union, intersection and A-only values.
        // METHOD SYNTAX PRACTICE
        var unionMethod = a.Union(b);
        var intersectionMethod = a.Intersect(b);
        var aOnlyMethod = a.Except(b);

        // QUERY SYNTAX PRACTICE
        var unionQuery =
            (from item in a select item).Union(
             from item in b select item);

        var intersectionQuery =
            (from item in a select item).Intersect(
             from item in b select item);

        var aOnlyQuery =
            (from item in a select item).Except(
             from item in b select item);

        Console.WriteLine($"\nTask 2: Union        - Method: {string.Join(", ", unionMethod)} | Query: {string.Join(", ", unionQuery)}");
        Console.WriteLine($"Task 2: Intersection - Method: {string.Join(", ", intersectionMethod)} | Query: {string.Join(", ", intersectionQuery)}");
        Console.WriteLine($"Task 2: A only       - Method: {string.Join(", ", aOnlyMethod)} | Query: {string.Join(", ", aOnlyQuery)}");
    }

    static void Exercise10_InnerJoin()
    {
        var employees = StoreData.Employees;
        var departments = StoreData.Departments;

        // METHOD SYNTAX PRACTICE
        var employeeDepartmentsMethod = employees.Join(
            departments,
            e => e.DepartmentId,
            d => d.DepartmentId,
            (e, d) => new
            {
                e.EmployeeName,
                d.DepartmentName
            });

        Console.WriteLine("\nMethod Syntax - Employees with departments (inner join)");
        foreach (var item in employeeDepartmentsMethod)
        {
            Console.WriteLine($"  {item.EmployeeName} - {item.DepartmentName}");
        }

        // QUERY SYNTAX PRACTICE
        var employeeDepartmentsQuery =
            from e in employees
            join d in departments
                on e.DepartmentId equals d.DepartmentId
            select new
            {
                e.EmployeeName,
                d.DepartmentName
            };

        Console.WriteLine("\nQuery Syntax - Employees with departments (inner join)");
        foreach (var item in employeeDepartmentsQuery)
        {
            Console.WriteLine($"  {item.EmployeeName} - {item.DepartmentName}");
        }
    }

    static void Exercise11_LeftJoin()
    {
        var employees = StoreData.Employees;
        var departments = StoreData.Departments;

        // METHOD SYNTAX PRACTICE
        var employeeDepartmentsMethod = employees
            .GroupJoin(
                departments,
                e => e.DepartmentId,
                d => d.DepartmentId,
                (e, matchingDepartments) => new
                {
                    Employee = e,
                    Departments = matchingDepartments
                })
            .SelectMany(
                x => x.Departments.DefaultIfEmpty(),
                (x, d) => new
                {
                    x.Employee.EmployeeName,
                    DepartmentName = d == null
                        ? "Unassigned"
                        : d.DepartmentName
                });

        Console.WriteLine("\nMethod Syntax - Employees with departments (left join)");
        foreach (var item in employeeDepartmentsMethod)
        {
            Console.WriteLine($"  {item.EmployeeName} - {item.DepartmentName}");
        }

        // QUERY SYNTAX PRACTICE
        var employeeDepartmentsQuery =
            from e in employees
            join d in departments
                on e.DepartmentId equals d.DepartmentId
                into departmentGroup
            from d in departmentGroup.DefaultIfEmpty()
            select new
            {
                e.EmployeeName,
                DepartmentName = d == null
                    ? "Unassigned"
                    : d.DepartmentName
            };

        Console.WriteLine("\nQuery Syntax - Employees with departments (left join)");
        foreach (var item in employeeDepartmentsQuery)
        {
            Console.WriteLine($"  {item.EmployeeName} - {item.DepartmentName}");
        }
    }

    static void Exercise12_GroupJoin()
    {
        var employees = StoreData.Employees;
        var departments = StoreData.Departments;

        // METHOD SYNTAX PRACTICE
        var departmentEmployeeCountsMethod = departments.GroupJoin(
            employees,
            d => d.DepartmentId,
            e => e.DepartmentId,
            (d, employeeGroup) => new
            {
                d.DepartmentName,
                EmployeeCount = employeeGroup.Count()
            });

        Console.WriteLine("\nMethod Syntax - Employee count per department");
        foreach (var item in departmentEmployeeCountsMethod)
        {
            Console.WriteLine($"  {item.DepartmentName}: {item.EmployeeCount}");
        }

        // QUERY SYNTAX PRACTICE
        var departmentEmployeeCountsQuery =
            from d in departments
            join e in employees
                on d.DepartmentId equals e.DepartmentId
                into employeeGroup
            select new
            {
                d.DepartmentName,
                EmployeeCount = employeeGroup.Count()
            };

        Console.WriteLine("\nQuery Syntax - Employee count per department");
        foreach (var item in departmentEmployeeCountsQuery)
        {
            Console.WriteLine($"  {item.DepartmentName}: {item.EmployeeCount}");
        }
    }

    static void Exercise13_Partitioning()
    {
        var products = StoreData.Products.OrderBy(p => p.ProductId);

        // Task 1: Get first three products.
        // METHOD SYNTAX PRACTICE
        var firstThreeMethod = products.Take(3);
        Show("Task 1: Method Syntax - First three products", firstThreeMethod.Select(p => p.ProductName));

        // QUERY SYNTAX PRACTICE
        var firstThreeQuery = (from p in products select p).Take(3);
        Show("Task 1: Query Syntax - First three products", firstThreeQuery.Select(p => p.ProductName));

        // Task 2: Skip first three products.
        // METHOD SYNTAX PRACTICE
        var remainingMethod = products.Skip(3);
        Show("Task 2: Method Syntax - Remaining products after skipping 3", remainingMethod.Select(p => p.ProductName));

        // QUERY SYNTAX PRACTICE
        var remainingQuery = (from p in products select p).Skip(3);
        Show("Task 2: Query Syntax - Remaining products after skipping 3", remainingQuery.Select(p => p.ProductName));
    }

    static void Exercise14_QuantifiersAndMembership()
    {
        List<int> requestedIds = new() { 1, 3, 5 };

        // Task: Find products whose IDs occur in requestedIds.
        // METHOD SYNTAX PRACTICE
        var methodResult = StoreData.Products
            .Where(p => requestedIds.Contains(p.ProductId));
        Show("Method Syntax - Products with IDs 1, 3, 5", methodResult.Select(p => $"{p.ProductId}: {p.ProductName}"));

        // QUERY SYNTAX PRACTICE
        var queryResult = from p in StoreData.Products
                          where requestedIds.Contains(p.ProductId)
                          select p;
        Show("Query Syntax - Products with IDs 1, 3, 5", queryResult.Select(p => $"{p.ProductId}: {p.ProductName}"));
    }

    static void Exercise15_StringAndNumericQueries()
    {
        var products = StoreData.Products;

        // Task 1: Find names starting with "O".
        // METHOD SYNTAX PRACTICE
        var methodStartsWith = products
            .Where(p => p.ProductName.StartsWith("O"));
        Show("Task 1: Method Syntax - Names starting with O", methodStartsWith.Select(p => p.ProductName));

        // QUERY SYNTAX PRACTICE
        var queryStartsWith = from p in products
                              where p.ProductName.StartsWith("O")
                              select p;
        Show("Task 1: Query Syntax - Names starting with O", queryStartsWith.Select(p => p.ProductName));

        // Task 2: Find products with even ProductIds.
        // METHOD SYNTAX PRACTICE
        var methodEvenIds = products
            .Where(p => p.ProductId % 2 == 0);
        Show("Task 2: Method Syntax - Even ProductIds", methodEvenIds.Select(p => $"{p.ProductId}: {p.ProductName}"));

        // QUERY SYNTAX PRACTICE
        var queryEvenIds = from p in products
                           where p.ProductId % 2 == 0
                           select p;
        Show("Task 2: Query Syntax - Even ProductIds", queryEvenIds.Select(p => $"{p.ProductId}: {p.ProductName}"));
    }

    static void Exercise16_NestedQueries()
    {
        var products = StoreData.Products;
        var orders = StoreData.Orders;

        // Task 1: Find products priced above average.
        // METHOD SYNTAX PRACTICE
        var averagePrice = products.Average(p => p.Price);
        Console.WriteLine($"\nAverage price: {averagePrice:F2}");

        var methodAboveAverage = products
            .Where(p => p.Price > averagePrice);
        Show("Task 1: Method Syntax - Above average price", methodAboveAverage.Select(p => $"{p.ProductName} - {p.Price}"));

        // QUERY SYNTAX PRACTICE
        var queryAboveAverage = from p in products
                                where p.Price > products.Average(x => x.Price)
                                select p;
        Show("Task 1: Query Syntax - Above average price", queryAboveAverage.Select(p => $"{p.ProductName} - {p.Price}"));

        // Task 2: Find customers with at least one order.
        // METHOD SYNTAX PRACTICE
        var methodCustomers = StoreData.Customers
            .Where(c => orders.Any(o => o.CustomerId == c.CustomerId));
        Show("Task 2: Method Syntax - Customers with orders", methodCustomers.Select(c => c.CustomerName));

        // QUERY SYNTAX PRACTICE
        var queryCustomers = from c in StoreData.Customers
                             where orders.Any(o => o.CustomerId == c.CustomerId)
                             select c;
        Show("Task 2: Query Syntax - Customers with orders", queryCustomers.Select(c => c.CustomerName));
    }

    static void Exercise17_CombinedQueries()
    {
        var products = StoreData.Products;

        // Task: Find in-stock Electronics products, sort by descending
        // price, and return only product names.
        // METHOD SYNTAX PRACTICE
        var methodResult = products
            .Where(p => p.Category == "Electronics" && p.Stock > 0)
            .OrderByDescending(p => p.Price)
            .Select(p => p.ProductName);
        Show("Method Syntax - In-stock Electronics, price descending", methodResult);

        // QUERY SYNTAX PRACTICE
        var queryResult = from p in products
                          where p.Category == "Electronics" && p.Stock > 0
                          orderby p.Price descending
                          select p.ProductName;
        Show("Query Syntax - In-stock Electronics, price descending", queryResult);
    }

    static void Exercise18_DictionaryAndLookup()
    {
        var products = StoreData.Products;

        // Task 1: Create a dictionary keyed by ProductId.
        // METHOD SYNTAX PRACTICE
        var methodDictionary = products.ToDictionary(p => p.ProductId);

        Console.WriteLine("\nTask 1: Method Syntax - Dictionary keyed by ProductId");
        foreach (var item in methodDictionary)
        {
            Console.WriteLine($"  {item.Key}: {item.Value.ProductName}");
        }

        // QUERY SYNTAX PRACTICE
        var queryDictionary = (from p in products select p)
            .ToDictionary(p => p.ProductId);

        Console.WriteLine("\nTask 1: Query Syntax - Dictionary keyed by ProductId");
        foreach (var item in queryDictionary)
        {
            Console.WriteLine($"  {item.Key}: {item.Value.ProductName}");
        }

        // Task 2: Create a lookup by Category.
        // METHOD SYNTAX PRACTICE
        var methodLookup = products.ToLookup(p => p.Category);

        Console.WriteLine("\nTask 2: Method Syntax - Lookup by Category");
        foreach (var group in methodLookup)
        {
            Console.WriteLine($"  {group.Key}");

            foreach (var p in group)
            {
                Console.WriteLine($"      {p.ProductName}");
            }
        }

        // QUERY SYNTAX PRACTICE
        var queryLookup = (from p in products select p)
            .ToLookup(p => p.Category);

        Console.WriteLine("\nTask 2: Query Syntax - Lookup by Category");
        foreach (var group in queryLookup)
        {
            Console.WriteLine($"  {group.Key}");

            foreach (var p in group)
            {
                Console.WriteLine($"      {p.ProductName}");
            }
        }
    }

    static void Exercise19_JoinOrdersAndCustomers()
    {
        var orders = StoreData.Orders;
        var customers = StoreData.Customers;

        // Task: Show OrderId, CustomerName, OrderDate and Status.
        // METHOD SYNTAX PRACTICE
        var methodResult = orders.Join(
            customers,
            o => o.CustomerId,
            c => c.CustomerId,
            (o, c) => new
            {
                o.OrderId,
                CustomerName = c.CustomerName,
                o.OrderDate,
                o.Status
            });

        Console.WriteLine("\nMethod Syntax - Orders with customer names");
        foreach (var item in methodResult)
        {
            Console.WriteLine(
                $"  {item.OrderId} | {item.CustomerName} | {item.OrderDate} | {item.Status}");
        }

        // QUERY SYNTAX PRACTICE
        var queryResult = from o in orders
                          join c in customers
                          on o.CustomerId equals c.CustomerId
                          select new
                          {
                              o.OrderId,
                              CustomerName = c.CustomerName,
                              o.OrderDate,
                              o.Status
                          };

        Console.WriteLine("\nQuery Syntax - Orders with customer names");
        foreach (var item in queryResult)
        {
            Console.WriteLine(
                $"  {item.OrderId} | {item.CustomerName} | {item.OrderDate} | {item.Status}");
        }
    }

    static void Exercise20_Reports()
    {
        var products = StoreData.Products;
        var orders = StoreData.Orders;

        // Task 1: Count orders by Status.
        // METHOD SYNTAX PRACTICE
        var methodStatusReport = orders
            .GroupBy(o => o.Status)
            .Select(g => new
            {
                Status = g.Key,
                Count = g.Count()
            });

        Console.WriteLine("\nTask 1: Method Syntax - Orders by status");
        foreach (var item in methodStatusReport)
        {
            Console.WriteLine($"  {item.Status}: {item.Count}");
        }

        // QUERY SYNTAX PRACTICE
        var queryStatusReport = from o in orders
                                group o by o.Status into g
                                select new
                                {
                                    Status = g.Key,
                                    Count = g.Count()
                                };

        Console.WriteLine("\nTask 1: Query Syntax - Orders by status");
        foreach (var item in queryStatusReport)
        {
            Console.WriteLine($"  {item.Status}: {item.Count}");
        }

        // Task 2: Create a category report with product count,
        // average price, total stock and inventory value.
        // METHOD SYNTAX PRACTICE
        var methodCategoryReport = products
            .GroupBy(p => p.Category)
            .Select(g => new
            {
                Category = g.Key,
                ProductCount = g.Count(),
                AveragePrice = g.Average(p => p.Price),
                TotalStock = g.Sum(p => p.Stock),
                InventoryValue = g.Sum(p => p.Price * p.Stock)
            });

        Console.WriteLine("\nTask 2: Method Syntax - Category report");
        foreach (var item in methodCategoryReport)
        {
            Console.WriteLine(
                $"  {item.Category} | Products: {item.ProductCount} | " +
                $"Average Price: {item.AveragePrice:F2} | " +
                $"Total Stock: {item.TotalStock} | " +
                $"Inventory Value: {item.InventoryValue:F2}");
        }

        // QUERY SYNTAX PRACTICE
        var queryCategoryReport = from p in products
                                  group p by p.Category into g
                                  select new
                                  {
                                      Category = g.Key,
                                      ProductCount = g.Count(),
                                      AveragePrice = g.Average(p => p.Price),
                                      TotalStock = g.Sum(p => p.Stock),
                                      InventoryValue = g.Sum(p => p.Price * p.Stock)
                                  };

        Console.WriteLine("\nTask 2: Query Syntax - Category report");
        foreach (var item in queryCategoryReport)
        {
            Console.WriteLine(
                $"  {item.Category} | Products: {item.ProductCount} | " +
                $"Average Price: {item.AveragePrice:F2} | " +
                $"Total Stock: {item.TotalStock} | " +
                $"Inventory Value: {item.InventoryValue:F2}");
        }
    }
}