using Microsoft.AspNetCore.Identity;

namespace ChickenWeb.Models
{
    public class Users : IdentityUser
    {
        public string FullName { get; set; }
        public DateTime RegisteredOn { get; set; } = DateTime.UtcNow;
    }
}
