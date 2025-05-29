using System.Security.Claims;
using SCED.API.Domain.Entity;

namespace SCED.API.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(User user);
        ClaimsPrincipal? ValidateToken(string token);
        string? GetUsernameFromToken(string token);
        long? GetUserIdFromToken(string token);
    }
}