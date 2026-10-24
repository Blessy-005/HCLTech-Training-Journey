namespace LinqMastery.Models;

public class Order
{
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; } = "";
    public decimal Discount { get; set; }
    public List<OrderItem> Items { get; set; } = new();
}
