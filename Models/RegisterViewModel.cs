using System.ComponentModel.DataAnnotations;


namespace ChickenWeb.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Name is required.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        //Add this regex:
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[!@#$%^&*()\-_=+{};:,<.>]).{8,}$",
            ErrorMessage = "Password must contain at least one uppercase letter and one special character.")]

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(40, MinimumLength = 8, ErrorMessage = "It should contain at least 8 characters.")]
        [DataType(DataType.Password)]
        [Compare("ConfirmPassword", ErrorMessage = "Password does not match")]
        public string Password { get; set; }

        
        [Required(ErrorMessage = "Confirm Password is required.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
