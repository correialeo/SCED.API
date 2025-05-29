
using SCED.API.Domain.Entity;

namespace SCED.API.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByIdAsync(long id);
        Task<User> CreateAsync(User user);
        Task<User> UpdateAsync(User user);
        Task<bool> DeleteAsync(long id);
        Task<IEnumerable<User>> GetAllAsync();
        Task<bool> ExistsAsync(string username);
    }
}