using SCED.API.Domain.Enums;

namespace SCED.API.DTO
{
    /// <summary>
    /// DTO para transferência de dados do usuário
    /// </summary>
    public class UserDTO
    {
        /// <summary>
        /// ID único do usuário
        /// </summary>
        /// <example>1</example>
        public long Id { get; set; }

        /// <summary>
        /// Nome de usuário
        /// </summary>
        /// <example>joao.silva</example>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Papel/função do usuário no sistema
        /// </summary>
        /// <example>Victim</example>
        public UserRole Role { get; set; }

        /// <summary>
        /// Necessidades específicas do usuário
        /// </summary>
        /// <example>Preciso de água potável e medicamentos</example>
        public string Necessities { get; set; } = string.Empty;

        /// <summary>
        /// Latitude da localização do usuário
        /// </summary>
        /// <example>-23.5505</example>
        public double Latitude { get; set; }

        /// <summary>
        /// Longitude da localização do usuário
        /// </summary>
        /// <example>-46.6333</example>
        public double Longitude { get; set; }
    }
}