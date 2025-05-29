using System.ComponentModel.DataAnnotations;
using SCED.API.Domain.Enums;

namespace SCED.API.Presentation.DTO
{
    /// <summary>
    /// Request para registro de novo usuário
    /// </summary>
    public class RegisterRequest
    {
        /// <summary>
        /// Nome de usuário único
        /// </summary>
        /// <example>maria.santos</example>
        [Required(ErrorMessage = "Username é obrigatório")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username deve ter entre 3 e 50 caracteres")]
        [RegularExpression(@"^[a-zA-Z0-9._-]+$", ErrorMessage = "Username pode conter apenas letras, números, pontos, hífens e underscores")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Senha forte do usuário
        /// </summary>
        /// <example>MinhaSenh@Segura123</example>
        [Required(ErrorMessage = "Password é obrigatório")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password deve ter entre 8 e 100 caracteres")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Papel/função do usuário no sistema
        /// </summary>
        /// <example>Victim</example>
        [Required(ErrorMessage = "Role é obrigatório")]
        [EnumDataType(typeof(UserRole), ErrorMessage = "Role deve ser um valor válido")]
        public UserRole Role { get; set; }

        /// <summary>
        /// Necessidades específicas do usuário (opcional)
        /// </summary>
        /// <example>Preciso de água potável e alimentos não perecíveis</example>
        [StringLength(1000, ErrorMessage = "Necessidades não podem exceder 1000 caracteres")]
        public string? Necessities { get; set; }

        /// <summary>
        /// Latitude da localização do usuário
        /// </summary>
        /// <example>-23.5505</example>
        [Range(-90.0, 90.0, ErrorMessage = "Latitude deve estar entre -90 e 90 graus")]
        public double Latitude { get; set; }

        /// <summary>
        /// Longitude da localização do usuário
        /// </summary>
        /// <example>-46.6333</example>
        [Range(-180.0, 180.0, ErrorMessage = "Longitude deve estar entre -180 e 180 graus")]
        public double Longitude { get; set; }
    }
}