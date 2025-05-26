using SCED.API.Domain.Enums;

namespace SCED.API.Domain.Entity
{
    public class User
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public UserRole Role { get; set; }
        public string? Necessities { get; set; } = string.Empty; // JSON string for user necessities
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public User() { }

        public User(string username, string passwordHash, UserRole role, double latitude, double longitude)
        {
            Username = username;
            PasswordHash = passwordHash;
            Role = role;
            Latitude = latitude;
            Longitude = longitude;
        }

        public User(string username, string passwordHash, UserRole role, string necessities, double latitude, double longitude)
        {
            Username = username;
            PasswordHash = passwordHash;
            Role = role;
            Necessities = necessities;
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
