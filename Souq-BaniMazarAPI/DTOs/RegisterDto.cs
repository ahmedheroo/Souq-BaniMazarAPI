namespace Souq_BaniMazarAPI.DTOs
{
    public class RegisterDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string PhoneNumber { get; set; }
        public IFormFile? NationalId { get; set; }
    }
}
