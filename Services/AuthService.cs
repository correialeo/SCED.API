using SCED.API.Domain.Entity;
using SCED.API.Domain.Interfaces;
using SCED.API.DTO;
using SCED.API.Interfaces;

namespace SCED.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public AuthService(IUserRepository userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetByUsernameAsync(request.Username);
            
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Credenciais inválidas");
            }

            var token = _tokenService.GenerateToken(user);
            
            return new LoginResponse
            {
                Token = token,
                Username = user.Username,
                Role = user.Role,
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            };
        }

        public async Task<UserDTO> RegisterAsync(RegisterRequest request)
        {
            if (await _userRepository.ExistsAsync(request.Username))
            {
                throw new InvalidOperationException("Username já existe");
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            
            var user = new User(
                request.Username,
                passwordHash,
                request.Role,
                request.Necessities ?? string.Empty,
                request.Latitude,
                request.Longitude
            );

            var createdUser = await _userRepository.CreateAsync(user);

            return new UserDTO
            {
                Id = createdUser.Id,
                Username = createdUser.Username,
                Role = createdUser.Role,
                Necessities = createdUser.Necessities,
                Latitude = createdUser.Latitude,
                Longitude = createdUser.Longitude
            };
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            return _tokenService.ValidateToken(token) != null;
        }

        public async Task<UserDTO?> GetUserFromTokenAsync(string token)
        {
            var userId = _tokenService.GetUserIdFromToken(token);
            if (userId == null) return null;

            var user = await _userRepository.GetByIdAsync(userId.Value);
            if (user == null) return null;

            return new UserDTO
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role,
                Necessities = user.Necessities,
                Latitude = user.Latitude,
                Longitude = user.Longitude
            };
        }
    }
}