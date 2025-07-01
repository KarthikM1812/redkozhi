using ChickenWeb.Models;

public class OrderSummaryViewModel
{
    public string Name { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public string Notes { get; set; }
    public DateTime OrderDate { get; set; }

    public List<CartItem> Items { get; set; }

}
