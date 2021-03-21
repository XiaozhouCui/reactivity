namespace API.DTOs
{
    // DTO: Data Transfer Object
    public class RegisterDto
    {
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
    }
}