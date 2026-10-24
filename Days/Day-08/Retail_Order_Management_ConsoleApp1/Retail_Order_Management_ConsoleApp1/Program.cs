using System;

string orderId;
int id;
Console.WriteLine("Enter Order ID:");
orderId = Console.ReadLine();

while (!int.TryParse(orderId, out id) || id <= 0)
{
    Console.WriteLine("Invalid Order ID. Please enter again:");
    orderId = Console.ReadLine();
}

Console.WriteLine("Enter Customer Name:");
string customerName = Console.ReadLine();

while (string.IsNullOrWhiteSpace(customerName))

{
    Console.WriteLine("Invalid input. Please enter valid Customer name.");
    customerName = Console.ReadLine();
}

Console.WriteLine("Enter product name:");
string productName = Console.ReadLine();
while (string.IsNullOrWhiteSpace(productName))
{
    Console.WriteLine("Invalid input. Please enter valid Product name.");
    productName = Console.ReadLine();

}
Console.WriteLine("Enter quantity:");
int quantity = int.Parse(Console.ReadLine());
while (quantity <= 0)
{
    Console.WriteLine("Invalid input. Quantity and price per unit must be positive values.");
    quantity = int.Parse(Console.ReadLine());
}
Console.WriteLine("Enter price per unit:");
decimal pricePerUnit = decimal.Parse(Console.ReadLine());
while (pricePerUnit <= 0)
{
    Console.WriteLine("Invalid input. Quantity and price per unit must be positive values.");
    pricePerUnit = decimal.Parse(Console.ReadLine());
}
int subtotal = quantity * (int)pricePerUnit;
string orderClassification;
if (subtotal < 1000)
{
    orderClassification = "Small order";
}
else if (subtotal >= 1000 && subtotal < 5000)
{
    orderClassification = "Medium order";
}
else if (subtotal >= 5000 && subtotal < 10000)
{
    orderClassification = "Large order";
}
else
{
    orderClassification = "Bulk order";
}
Console.WriteLine("=====================================");
Console.WriteLine($"Order Id : {orderId}");
Console.WriteLine($"Customer Name : {customerName}");
Console.WriteLine($"Product Name : {productName}");
Console.WriteLine($"Quantity : {quantity}");
Console.WriteLine($"Price Per Unit : {pricePerUnit}");
Console.WriteLine($"Subtotal : {subtotal}");
Console.WriteLine($"Order Classification : {orderClassification}");
Console.WriteLine("=====================================");

