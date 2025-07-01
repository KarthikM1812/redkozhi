public class OrderItem
{
    public int Id { get; set; }

  

    // Customer details
    public string Name { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public string Notes { get; set; }

    // Item details
    public string ItemName { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string UserEmail { get; set; }

    public DateTime OrderedAt { get; set; } = DateTime.Now;
}
