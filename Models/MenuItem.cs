using System.ComponentModel.DataAnnotations;

namespace ChickenWeb.Models
{
    public class MenuItem
    {
        public int Id { get; set; }

        public string Name { get; set; }      // Ex: Zinger Burger

        public string Image { get; set; } = string.Empty;

        public string Spices { get; set; }    // Ex: Spicy, Cheesy

        [Required]
        [Range(1, 10000)]
        public decimal Price { get; set; }    // Ex: ₹149.00

        public string Type { get; set; }

        //public string? ImagePath { get; set; }
        //public string image { get; set; }
    }
}
