using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SCED.API.Domain.Entity;
using SCED.API.Interfaces;

namespace SCED.API.Services
{
    public class TokenService : ITokenService
    {
        private readonly string _secretKey;
        private readonly IConfiguration _configuration;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly ILogger<TokenService> _logger;

        public TokenService(IConfiguration configuration, ILogger<TokenService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _secretKey = _configuration["Jwt:SecretKey"] ?? throw new ArgumentNullException("Jwt:SecretKey não configurado");
            _issuer = _configuration["Jwt:Issuer"] ?? "SCED.API";
            _audience = _configuration["Jwt:Audience"] ?? "SCED.API.Client";

            if (_secretKey.Length < 32)
            {
                throw new ArgumentException("Jwt:SecretKey deve ter pelo menos 32 caracteres");
            }
        }

        public string GenerateToken(User user)
        {
            try
            {
                JwtSecurityTokenHandler? tokenHandler = new JwtSecurityTokenHandler();
                byte[] key = Encoding.UTF8.GetBytes(_secretKey);

                List<Claim> claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role.ToString()),
                    new Claim("UserId", user.Id.ToString()),
                    new Claim("latitude", user.Latitude.ToString("F6")),
                    new Claim("longitude", user.Longitude.ToString("F6")),
                    new Claim("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                if (!string.IsNullOrEmpty(user.Necessities))
                {
                    claims.Add(new Claim("necessities", user.Necessities));
                }

                SecurityTokenDescriptor? tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddHours(24),
                    Issuer = _issuer,
                    Audience = _audience,
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
                };

                SecurityToken? token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar token para usuário {UserId}", user.Id);
                throw new Exception("Erro interno ao gerar token");
            }
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            try
            {
                JwtSecurityTokenHandler? tokenHandler = new JwtSecurityTokenHandler();
                byte[] key = Encoding.UTF8.GetBytes(_secretKey);

                TokenValidationParameters? validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(5),
                    RequireExpirationTime = true,
                    RequireSignedTokens = true
                };

                ClaimsPrincipal? principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                
                if (validatedToken is not JwtSecurityToken jwtToken || 
                    !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }

                return principal;
            }
            catch (SecurityTokenExpiredException)
            {
                _logger.LogWarning("Token expirado");
                return null;
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogWarning(ex, "Token inválido");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado na validação do token");
                return null;
            }
        }
        
        public string? GetUsernameFromToken(string token)
        {
            ClaimsPrincipal? principal = ValidateToken(token);
            return principal?.FindFirst(ClaimTypes.Name)?.Value;
        }

        public long? GetUserIdFromToken(string token)
        {
            ClaimsPrincipal? principal = ValidateToken(token);
            string? userIdClaim = principal?.FindFirst("UserId")?.Value ?? principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return long.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }
}