using SCED.API.Domain.Enums;

namespace SCED.API.Presentation.DTO
{
    /// <summary>
    /// Response do login com token JWT
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// Token JWT para autenticação
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Nome de usuário
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Papel/função do usuário no sistema
        /// </summary>
        public UserRole Role { get; set; }

        /// <summary>
        /// Data/hora de expiração do token
        /// </summary>
        public DateTime ExpiresAt { get; set; }
    }
}