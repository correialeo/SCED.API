using System.ComponentModel.DataAnnotations;

namespace SCED.API.DTO
{
    /// <summary>
    /// Request para login de usuário
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Nome de usuário
        /// </summary>
        /// <example>joao.silva</example>
        [Required(ErrorMessage = "Username é obrigatório")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username deve ter entre 3 e 50 caracteres")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Senha do usuário
        /// </summary>
        /// <example>MinhaSenh@123</example>
        [Required(ErrorMessage = "Password é obrigatório")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password deve ter entre 8 e 100 caracteres")]
        public string Password { get; set; } = string.Empty;
    }
}