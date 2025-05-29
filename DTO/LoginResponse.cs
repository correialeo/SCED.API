using SCED.API.Domain.Enums;

namespace SCED.API.DTO
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public string Username { get; set; }
        public UserRole Role { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}