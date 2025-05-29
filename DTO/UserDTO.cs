using SCED.API.Domain.Enums;

namespace SCED.API.DTO
{
    public class UserDTO
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public UserRole Role { get; set; }
        public string? Necessities { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}