using ChickenWeb.Models;
using System.ComponentModel.DataAnnotations.Schema;
[NotMapped]
public class OrderRequest
{
   
    public string Name { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public string Notes { get; set; }

    //public DateTime OrderDate { get; set; } = DateTime.Now;
    public List<CartItem> Cart{ get; set; }

}


