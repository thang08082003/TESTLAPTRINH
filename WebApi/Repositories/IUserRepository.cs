using SharedLib.Models;

namespace WebApi.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByCodeAsync(string code);
        Task<User> CreateAsync(User user);
        Task<User> UpdateAsync(User user);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id); 
        Task<bool> CodeExistsAsync(string code, int? excludeId = null);
        Task<bool> EmailExistsAsync(string email, int? excludeId = null);
    }
}
