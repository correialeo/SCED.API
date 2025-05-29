using SCED.API.Application.Interfaces;
using SCED.API.Domain.Entity;
using SCED.API.Domain.Interfaces;
using SCED.API.Presentation.DTO;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace SCED.API.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IUserRepository userRepository, ITokenService tokenService, ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username))
            {
                throw new ArgumentException("Username é obrigatório");
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                throw new ArgumentException("Password é obrigatório");
            }

            try
            {
                User? user = await _userRepository.GetByUsernameAsync(request.Username.Trim().ToLower());
                
                if (user == null)
                {
                    _logger.LogWarning("Tentativa de login com username inexistente: {Username}", request.Username);
                    throw new UnauthorizedAccessException("Credenciais inválidas");
                }

                if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    _logger.LogWarning("Tentativa de login com senha incorreta para usuário: {Username}", request.Username);
                    throw new UnauthorizedAccessException("Credenciais inválidas");
                }

                string? token = _tokenService.GenerateToken(user);
                
                _logger.LogInformation("Login realizado com sucesso para usuário: {Username}", user.Username);
                
                return new LoginResponse
                {
                    Token = token,
                    Username = user.Username,
                    Role = user.Role,
                    ExpiresAt = DateTime.UtcNow.AddHours(24)
                };
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante processo de login para usuário: {Username}", request.Username);
                throw new Exception("Erro interno durante o login");
            }
        }

        public async Task<UserDTO> RegisterAsync(RegisterRequest request)
        {
            ValidateRegisterRequest(request);

            try
            {
                if (await _userRepository.ExistsAsync(request.Username.Trim().ToLower()))
                {
                    throw new InvalidOperationException("Username já existe");
                }

                string? passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, 12);
                
                User? user = new User(
                    request.Username.Trim().ToLower(),
                    passwordHash,
                    request.Role,
                    request.Necessities?.Trim() ?? string.Empty,
                    request.Latitude,
                    request.Longitude
                );

                User? createdUser = await _userRepository.CreateAsync(user);
                
                _logger.LogInformation("Usuário criado com sucesso: {Username}", createdUser.Username);

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
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante criação de usuário: {Username}", request.Username);
                throw new Exception("Erro interno durante o registro");
            }
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return false;

            try
            {
                ClaimsPrincipal? principal = _tokenService.ValidateToken(token);
                return principal != null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Erro durante validação de token");
                return false;
            }
        }

        public async Task<UserDTO?> GetUserFromTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            try
            {
                long? userId = _tokenService.GetUserIdFromToken(token);
                if (userId == null) return null;

                User? user = await _userRepository.GetByIdAsync(userId.Value);
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter usuário do token");
                return null;
            }
        }

        private void ValidateRegisterRequest(RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username))
            {
                throw new ArgumentException("Username é obrigatório");
            }

            if (request.Username.Length < 3 || request.Username.Length > 50)
            {
                throw new ArgumentException("Username deve ter entre 3 e 50 caracteres");
            }

            if (!Regex.IsMatch(request.Username, @"^[a-zA-Z0-9._-]+$"))
            {
                throw new ArgumentException("Username pode conter apenas letras, números, pontos, hífens e underscores");
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                throw new ArgumentException("Password é obrigatório");
            }

            if (!IsValidPassword(request.Password))
            {
                throw new ArgumentException("Password deve ter pelo menos 8 caracteres, incluindo pelo menos 1 letra maiúscula, 1 minúscula, 1 número e 1 caractere especial");
            }

            if (request.Latitude != 0 && (request.Latitude < -90 || request.Latitude > 90))
            {
                throw new ArgumentException("Latitude deve estar entre -90 e 90 graus");
            }

            if (request.Longitude != 0 && (request.Longitude < -180 || request.Longitude > 180))
            {
                throw new ArgumentException("Longitude deve estar entre -180 e 180 graus");
            }

            if (!string.IsNullOrWhiteSpace(request.Necessities) && request.Necessities.Length > 1000)
            {
                throw new ArgumentException("Necessidades não podem exceder 1000 caracteres");
            }
        }

        private bool IsValidPassword(string password)
        {
            if (password.Length < 8)
                return false;

            if (!Regex.IsMatch(password, @"[A-Z]"))
                return false;

            if (!Regex.IsMatch(password, @"[a-z]"))
                return false;

            if (!Regex.IsMatch(password, @"\d"))
                return false;

            if (!Regex.IsMatch(password, @"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]"))
                return false;

            return true;
        }
    }
}