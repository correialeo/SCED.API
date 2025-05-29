using SCED.API.Domain.Enums;

namespace SCED.API.DTO
{
    public class RegisterRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public UserRole Role { get; set; }
        public string? Necessities { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}