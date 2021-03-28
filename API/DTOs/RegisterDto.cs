using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    // DTO: Data Transfer Object
    public class RegisterDto
    {
        // use data annotation to validate user inputs
        [Required]
        public string DisplayName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [RegularExpression("(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{4,8}$", ErrorMessage = "Password must be complex")]
        public string Password { get; set; }

        [Required]
        public string Username { get; set; }
    }
}