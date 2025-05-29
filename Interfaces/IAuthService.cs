using SCED.API.DTO;

namespace SCED.API.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<UserDTO> RegisterAsync(RegisterRequest request);
        Task<bool> ValidateTokenAsync(string token);
        Task<UserDTO?> GetUserFromTokenAsync(string token);
    }

}