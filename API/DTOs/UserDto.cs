namespace API.DTOs
{
    // DTO: Data Transfer Object
    public class UserDto
    {
        // Data to be send back to client after successful login
        public string DisplayName { get; set; }
        public string Token { get; set; }
        public string Username { get; set; }
        public string Image { get; set; }
    }
}